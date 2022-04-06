using CloudNative.CloudEvents.Core;
using CloudNative.CloudEvents.NewtonsoftJson;
using FaasNet.EventStore;
using FaasNet.Lock;
using FaasNet.StateMachine.Runtime;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Serializer;
using FaasNet.StateMachine.Worker.Persistence;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace FaasNet.StateMachine.Worker
{
    public class EventConsumerStore : IEventConsumerStore
    {
        private readonly IVpnSubscriptionRepository _vpnSubscriptionRepository;
        private readonly ICloudEventSubscriptionRepository _cloudEventSubscriptionRepository;
        private readonly ICommitAggregateHelper _commitAggregateHelper;
        private readonly IEnumerable<IMessageListener> _messageListeners;
        private readonly IDistributedLock _distributedLock;
        private readonly IRuntimeEngine _runtimeEngine;
        private readonly ICollection<MessageListenerRecord> _listeners;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isInitialized = false;

        public EventConsumerStore(IVpnSubscriptionRepository vpnSubscriptionRepository, ICloudEventSubscriptionRepository cloudEventSubscriptionRepository, ICommitAggregateHelper commitAggregateHelper, IDistributedLock distributedLock, IRuntimeEngine runtimeEngine, IEnumerable<IMessageListener> messageListeners)
        {
            _vpnSubscriptionRepository = vpnSubscriptionRepository;
            _cloudEventSubscriptionRepository = cloudEventSubscriptionRepository;
            _commitAggregateHelper = commitAggregateHelper;
            _distributedLock = distributedLock;
            _messageListeners = messageListeners;
            _runtimeEngine = runtimeEngine;
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
            var vpns = await _vpnSubscriptionRepository.GetAll(cancellationToken);
            foreach (var vpn in vpns)
            {
                await ListenVpn(vpn.Vpn, cancellationToken);
            }

            _isInitialized = true;
        }

        public bool IsListeningVpn(string vpn)
        {
            return _listeners.Any(l => l.Vpn == vpn);
        }

        public async Task ListenVpn(string vpn, CancellationToken cancellationToken)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("EventConsumer is not initialized");
            }

            foreach (var messageListener in _messageListeners)
            {
                if (_listeners.Any(l => l.Name == messageListener.Name && !l.SupportVpn))
                {
                    continue;
                }

                var r = await messageListener.Listen(vpn, HandleMessage, cancellationToken);
                _listeners.Add(new MessageListenerRecord(vpn, r, messageListener.SupportVpn, messageListener.Name));
            }
        }

        public void Stop()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("EventConsumer is not initialized");
            }

            _isInitialized = false;
            _cancellationTokenSource.Cancel();
            foreach (var l in _listeners)
            {
                l.Listener.Stop();
            }

            _listeners.Clear();
        }

        public void Dispose()
        {
            if (!_isInitialized)
            {
                Stop();
            }
        }

        #endregion

        private async void HandleMessage(MessageResult obj)
        {
            var serializer = new RuntimeSerializer();
            var jsonEventFormatter = new JsonEventFormatter();
            if (await _distributedLock.TryAcquireLock($"externalevts-{obj.Vpn}", _cancellationTokenSource.Token))
            {
                var cloudEvtMessage = obj.Content.First();
                var vpnSubscriptions = _cloudEventSubscriptionRepository.Query().Where(e => e.Vpn == obj.Vpn && !e.IsConsumed).ToList();
                var lst = vpnSubscriptions.Where(vs => obj.Content.Any(ce => ce.Type == vs.Type && ce.Source.ToString() == vs.Source)).GroupBy(k => k.WorkflowInstanceId);
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var kvp in lst)
                    {
                        var stateMachineInstanceId = kvp.Key;
                        var stateMachineInstance = await _commitAggregateHelper.Get<StateMachineInstanceAggregate>(stateMachineInstanceId, _cancellationTokenSource.Token);
                        var stateMachineDef = serializer.DeserializeYaml(stateMachineInstance.SerializedDefinition);
                        foreach (var subscription in kvp)
                        {
                            var firstMsg = obj.Content.First(ce => ce.Type == subscription.Type && ce.Source.ToString() == subscription.Source);
                            if (stateMachineInstance.TryConsumeEvt(subscription.StateInstanceId, subscription.Source, subscription.Type, firstMsg.Data.ToString()))
                            {
                                byte[] payload = BinaryDataUtilities.AsArray(jsonEventFormatter.EncodeBinaryModeEventData(firstMsg));
                                var json = Encoding.UTF8.GetString(payload);
                                await _runtimeEngine.Launch(stateMachineDef, stateMachineInstance, JObject.Parse(json), subscription.StateInstanceId, CancellationToken.None);
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
        }

        private class MessageListenerRecord
        {
            public MessageListenerRecord(string vpn, IMessageListenerResult listener, bool supportVpn, string name)
            {
                Vpn = vpn;
                Listener = listener;
                SupportVpn = supportVpn;
                Name = name;
            }

            public string Vpn { get; set; }
            public IMessageListenerResult Listener { get; set; }
            public bool SupportVpn { get; set; }
            public string Name { get; set; }
        }
    }
}