using FaasNet.EventStore;
using FaasNet.Lock;
using FaasNet.StateMachine.Runtime;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Serializer;
using FaasNet.StateMachine.Worker.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace FaasNet.StateMachine.Worker
{
    public class EventConsumerStore : IEventConsumerStore
    {
        private readonly ICloudEventSubscriptionRepository _cloudEventSubscriptionRepository;
        private readonly ICommitAggregateHelper _commitAggregateHelper;
        private readonly IEnumerable<IMessageListener> _messageListeners;
        private readonly IDistributedLock _distributedLock;
        private readonly IRuntimeEngine _runtimeEngine;
        private readonly ILogger<EventConsumerStore> _logger;
        private readonly ICollection<MessageListenerRecord> _listeners;
        private readonly StateMachineWorkerOptions _workerOptions;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isInitialized = false;

        public EventConsumerStore(ICloudEventSubscriptionRepository cloudEventSubscriptionRepository, ICommitAggregateHelper commitAggregateHelper, IDistributedLock distributedLock, IRuntimeEngine runtimeEngine, IEnumerable<IMessageListener> messageListeners, ILogger<EventConsumerStore> logger, IOptions<StateMachineWorkerOptions> options)
        {
            _cloudEventSubscriptionRepository = cloudEventSubscriptionRepository;
            _commitAggregateHelper = commitAggregateHelper;
            _distributedLock = distributedLock;
            _messageListeners = messageListeners;
            _runtimeEngine = runtimeEngine;
            _logger = logger;
            _workerOptions = options.Value;
            _listeners = new List<MessageListenerRecord>();
        }

        #region Public Methods

        public async Task Init(CancellationToken cancellationToken)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException("EventConsumer is already initialized");
            }

            _cancellationTokenSource = new CancellationTokenSource();
            foreach (var messageListener in _messageListeners)
            {
                var result = await messageListener.Listen(HandleMessage, cancellationToken);
                _listeners.Add(new MessageListenerRecord(result, messageListener.Name));
            }

            _isInitialized = true;
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("EventConsumer is not initialized");
            }

            foreach (var l in _listeners)
            {
                await l.Listener.Stop(cancellationToken);
            }

            _listeners.Clear();
            _cancellationTokenSource.Cancel();
            _isInitialized = false;
        }

        public void Dispose()
        {
            Dispose(CancellationToken.None);
        }

        public async void Dispose(CancellationToken cancellationToken)
        {
            if (!_isInitialized)
            {
                await Stop(cancellationToken);
            }
        }

        #endregion

        private async Task HandleMessage(MessageResult obj)
        {
            var serializer = new RuntimeSerializer();
            if (await _distributedLock.TryAcquireLock("externalevts", _cancellationTokenSource.Token))
            {
                try
                {
                    var cloudEvtMessage = obj.Content.First();
                    var splittedTopic = obj.TopicMessage.Split('/');
                    var expectedRootTopic = splittedTopic.First();
                    var expectedMessageTopic = splittedTopic.Last();
                    var nbActiveSubscriptions = await _cloudEventSubscriptionRepository.NbActiveSubscriptions(_cancellationTokenSource.Token);
                    StateMachineRuntimeMeter.SetActiveSubscriptions(nbActiveSubscriptions, _workerOptions.WorkerName);
                    var vpnSubscriptions = _cloudEventSubscriptionRepository.Query().Where(e => e.Vpn == obj.Vpn && e.Topic == expectedMessageTopic && e.RootTopic == expectedRootTopic && !e.IsConsumed).ToList();
                    var lst = vpnSubscriptions.Where(vs => obj.Content.Any(ce => ce.Type == vs.Type && ce.Source.ToString() == vs.Source)).GroupBy(k => k.WorkflowInstanceId);
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        foreach (var kvp in lst)
                        {
                            var stateMachineInstanceId = kvp.Key;
                            var stateMachineInstance = await _commitAggregateHelper.Get<StateMachineInstanceAggregate>(stateMachineInstanceId, _cancellationTokenSource.Token);
                            if (string.IsNullOrWhiteSpace(stateMachineInstance.Id))
                            {
                                continue;
                            }

                            var stateMachineDef = serializer.DeserializeYaml(stateMachineInstance.SerializedDefinition);
                            _logger.LogInformation("State machine instance {stateMachineInstanceId} consume the event, RootTopic = {rootTopic}, MessageTopic = {messageTopic}", stateMachineInstance.Id, expectedRootTopic, expectedMessageTopic);
                            foreach (var subscription in kvp)
                            {
                                var firstMsg = obj.Content.First(ce => ce.Type == subscription.Type && ce.Source.ToString() == subscription.Source);
                                var jObj = JObject.Parse(firstMsg.Data.ToString());
                                if (stateMachineInstance.TryConsumeEvt(subscription.StateInstanceId, subscription.Source, subscription.Type, jObj.ToString()))
                                {
                                    await _runtimeEngine.Launch(stateMachineDef, stateMachineInstance, jObj, subscription.StateInstanceId, CancellationToken.None);
                                    await _commitAggregateHelper.Commit(stateMachineInstance, _cancellationTokenSource.Token);
                                }

                                subscription.IsConsumed = true;
                            }
                        }

                        await _cloudEventSubscriptionRepository.Update(lst.SelectMany(_ => _), _cancellationTokenSource.Token);
                        await _cloudEventSubscriptionRepository.SaveChanges(_cancellationTokenSource.Token);
                        scope.Complete();
                    }
                }
                finally
                {
                    await _distributedLock.ReleaseLock("externalevts", _cancellationTokenSource.Token);
                }
            }
        }

        private class MessageListenerRecord
        {
            public MessageListenerRecord(IMessageListenerResult listener,  string name)
            {
                Listener = listener;
                Name = name;
            }

            public IMessageListenerResult Listener { get; set; }
            public string Name { get; set; }
        }
    }
}