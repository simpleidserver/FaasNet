using FaasNet.StateMachine.Runtime.CloudEvent.Models;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Domains.Subscriptions;
using FaasNet.StateMachine.Runtime.Infrastructure;
using FaasNet.StateMachine.Runtime.Persistence;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace FaasNet.StateMachine.Runtime.Consumers
{
    public class CloudEventConsumer : IConsumer<CloudEventMessage>
    {
        private readonly ICloudEventSubscriptionRepository _cloudEventSubscriptionRepository;
        private readonly IStateMachineInstanceRepository _workflowInstanceRepository;
        private readonly IRuntimeEngine _runtimeEngine;
        private readonly IStateMachineDefinitionRepository _workflowDefinitionRepository;
        private readonly IDistributedLock _distributedLock;
        private readonly ILogger<CloudEventConsumer> _logger;

        public CloudEventConsumer(
            ICloudEventSubscriptionRepository cloudEventSubscriptionRepository, 
            IStateMachineInstanceRepository workflowInstanceRepository, 
            IRuntimeEngine runtimeEngine, 
            IStateMachineDefinitionRepository workflowDefinitionRepository, 
            IDistributedLock distributedLock, 
            ILogger<CloudEventConsumer> logger)
        {
            _cloudEventSubscriptionRepository = cloudEventSubscriptionRepository;
            _workflowInstanceRepository = workflowInstanceRepository;
            _runtimeEngine = runtimeEngine;
            _workflowDefinitionRepository = workflowDefinitionRepository;
            _distributedLock = distributedLock;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CloudEventMessage> context)
        {
            if (!(await _distributedLock.TryAcquireLock(context.Message.UniqueId, CancellationToken.None)))
            {
                _logger.LogError("The event cannot be consumed because it's a duplicate");
                return;
            }

            try
            {
                var subscriptions = GetSubscriptions(context);
                foreach(var subscription in subscriptions)
                {
                    var workflowDef = _workflowDefinitionRepository.Query().FirstOrDefault(w => w.Id == subscription.WorkflowInstance.WorkflowDefId);
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        foreach (var sub in subscription.Subscriptions)
                        {
                            subscription.WorkflowInstance.ConsumeEvent(sub.StateInstanceId, context.Message.Source, context.Message.Type, context.Message.Data?.ToString());
                            await _runtimeEngine.Launch(workflowDef, subscription.WorkflowInstance, context.Message.Data, sub.StateInstanceId, CancellationToken.None);
                            sub.IsConsumed = true; 
                            await _workflowInstanceRepository.Update(subscription.WorkflowInstance, CancellationToken.None);
                            await _cloudEventSubscriptionRepository.Update(new List<CloudEventSubscriptionAggregate> { sub }, CancellationToken.None);
                        }

                        await _workflowInstanceRepository.SaveChanges(CancellationToken.None);
                        await _cloudEventSubscriptionRepository.SaveChanges(CancellationToken.None);
                        scope.Complete();
                    }
                }
            }
            finally
            {
                await _distributedLock.ReleaseLock(context.Message.UniqueId, CancellationToken.None);
            }
        }

        protected virtual List<SubscriptionResult> GetSubscriptions(ConsumeContext<CloudEventMessage> context)
        {
            var subscriptions = _cloudEventSubscriptionRepository.Query().Where(c => c.Source == context.Message.Source && c.Type == context.Message.Type && !c.IsConsumed).ToList();
            var result = new List<SubscriptionResult>();
            foreach (var kvp in subscriptions.GroupBy(s => s.WorkflowInstanceId))
            {
                var workflowInstance = _workflowInstanceRepository.Query().First(w => w.Id == kvp.Key);
                result.Add(new SubscriptionResult
                {
                    Subscriptions = kvp.ToList(),
                    WorkflowInstance = workflowInstance
                });
            }

            return result;
        }

        protected class SubscriptionResult
        {
            public StateMachineInstanceAggregate WorkflowInstance { get; set; }
            public ICollection<CloudEventSubscriptionAggregate> Subscriptions { get; set; }
        }
    }
}
