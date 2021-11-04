namespace FaasNet.Runtime.Domains
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
    }
}
