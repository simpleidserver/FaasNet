using System;

namespace FaasNet.StateMachine.Runtime.Domains.Subscriptions
{
    public class CloudEventSubscriptionAggregate: ICloneable
    {
        public CloudEventSubscriptionAggregate()
        {
            IsConsumed = false;
        }

        public string WorkflowInstanceId { get; set; }
        public string StateInstanceId { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public bool IsConsumed { get; set; }

        public static CloudEventSubscriptionAggregate Create(string workflowInstanceId, string stateInstanceId, string source, string type)
        {
            return new CloudEventSubscriptionAggregate
            {
                Source = source,
                StateInstanceId = stateInstanceId,
                Type = type,
                WorkflowInstanceId = workflowInstanceId
            };
        }

        public object Clone()
        {
            return new CloudEventSubscriptionAggregate
            {
                IsConsumed = IsConsumed,
                WorkflowInstanceId= WorkflowInstanceId,
                Source = Source,
                StateInstanceId = StateInstanceId,
                Type = Type
            };
        }
    }
}
