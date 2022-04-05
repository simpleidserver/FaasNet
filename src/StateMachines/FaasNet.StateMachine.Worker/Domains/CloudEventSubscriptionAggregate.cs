namespace FaasNet.StateMachine.Worker.Domains
{
    public class CloudEventSubscriptionAggregate
    {
        public CloudEventSubscriptionAggregate()
        {
            IsConsumed = false;
        }

        public string WorkflowInstanceId { get; set; }
        public string StateInstanceId { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public string Vpn { get; set; }
        public bool IsConsumed { get; set; }

        public static CloudEventSubscriptionAggregate Create(string workflowInstanceId, string stateInstanceId, string source, string type, string vpn)
        {
            return new CloudEventSubscriptionAggregate
            {
                Source = source,
                StateInstanceId = stateInstanceId,
                Type = type,
                WorkflowInstanceId = workflowInstanceId,
                Vpn = vpn
            };
        }

        public object Clone()
        {
            return new CloudEventSubscriptionAggregate
            {
                IsConsumed = IsConsumed,
                WorkflowInstanceId = WorkflowInstanceId,
                Source = Source,
                StateInstanceId = StateInstanceId,
                Type = Type,
                Vpn = Vpn
            };
        }
    }
}