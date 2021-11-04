using FaasNet.Runtime.CloudEvent.Models;
using FaasNet.Runtime.Domains;
using FaasNet.Runtime.Persistence;
using MassTransit;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Consumers
{
    public class CloudEventConsumer : IConsumer<CloudEventMessage>
    {
        private readonly ICloudEventSubscriptionRepository _cloudEventSubscriptionRepository;
        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
        private readonly IRuntimeEngine _runtimeEngine;
        private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;

        public CloudEventConsumer(ICloudEventSubscriptionRepository cloudEventSubscriptionRepository, IWorkflowInstanceRepository workflowInstanceRepository, IRuntimeEngine runtimeEngine, IWorkflowDefinitionRepository workflowDefinitionRepository)
        {
            _cloudEventSubscriptionRepository = cloudEventSubscriptionRepository;
            _workflowInstanceRepository = workflowInstanceRepository;
            _runtimeEngine = runtimeEngine;
            _workflowDefinitionRepository = workflowDefinitionRepository;
        }

        public async Task Consume(ConsumeContext<CloudEventMessage> context)
        {
            var subscriptions = _cloudEventSubscriptionRepository.Query().Where(c => c.Source == context.Message.Source && c.Type == context.Message.Type && !c.IsConsumed).ToList();
            var dic = new Dictionary<WorkflowInstanceAggregate, List<string>>();
            foreach(var kvp in subscriptions.GroupBy(s => s.WorkflowInstanceId))
            {
                var workflowInstance = _workflowInstanceRepository.Query().First(w => w.Id == kvp.Key);
                foreach (var subscription in kvp)
                {
                    subscription.IsConsumed = true;
                    workflowInstance.ConsumeEvent(subscription.StateInstanceId, context.Message.Source, context.Message.Type, context.Message.Data?.ToString());
                }

                dic.Add(workflowInstance, kvp.Select(kvp => kvp.StateInstanceId).ToList());
            }

            await _cloudEventSubscriptionRepository.Update(subscriptions, CancellationToken.None);
            await _cloudEventSubscriptionRepository.SaveChanges(CancellationToken.None);
            foreach(var kvp in dic)
            {
                var workflowDef = _workflowDefinitionRepository.Query().FirstOrDefault(w => w.Id == kvp.Key.WorkflowDefId);
                foreach(var value in kvp.Value)
                {
                    await _runtimeEngine.Launch(workflowDef, kvp.Key, JObject.Parse("{}"), value , CancellationToken.None);
                }
            }
        }
    }
}
