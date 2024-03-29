﻿namespace FaasNet.StateMachine.Worker.Domains
{
    public class CloudEventSubscriptionAggregate
    {
        public CloudEventSubscriptionAggregate()
        {
            IsConsumed = false;
        }

        public string WorkflowInstanceId { get; set; }
        public string StateInstanceId { get; set; }
        public string RootTopic { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public string Vpn { get; set; }
        public string Topic { get; set; }
        public bool IsConsumed { get; set; }

        public static CloudEventSubscriptionAggregate Create(string workflowInstanceId, string stateInstanceId, string rootTopic, string source, string type, string vpn, string topic)
        {
            return new CloudEventSubscriptionAggregate
            {
                Source = source,
                StateInstanceId = stateInstanceId,
                RootTopic = rootTopic,
                Type = type,
                WorkflowInstanceId = workflowInstanceId,
                Vpn = vpn,
                Topic = topic
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
                RootTopic = RootTopic,
                Vpn = Vpn,
                Topic = Topic
            };
        }
    }
}