using CloudNative.CloudEvents;
using Confluent.Kafka;
using System;

namespace EventMesh.Runtime.Kafka
{
    public static class ConsumeResultExtensions
    {
        public static CloudEvent ToCloudEvent(this ConsumeResult<Ignore, string> message,
            CloudEventFormatter cloudEventFormatter,
            string source,
            string topicName,
            params CloudEvent[]? extensionAttributes)
        {
            var cloudEvent = new CloudEvent();
            cloudEvent.Id = Guid.NewGuid().ToString();
            cloudEvent.Source = new Uri($"{source}:{topicName}");
            cloudEvent.Type = message.Topic;
            cloudEvent.DataContentType = "application/json";
            cloudEvent.Data = message.Message.Value;
            return cloudEvent;
        }
    }
}
