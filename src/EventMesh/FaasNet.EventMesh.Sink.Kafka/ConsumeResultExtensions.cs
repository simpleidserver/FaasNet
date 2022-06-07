using CloudNative.CloudEvents;
using CloudNative.CloudEvents.Core;
using CloudNative.CloudEvents.Extensions;
using Confluent.Kafka;
using System;
using System.Text;

namespace FaasNet.EventMesh.Sink.Kafka
{
    public static class ConsumeResultExtensions
    {
        internal const string KafkaHeaderPrefix = "cloudEvents:";
        internal const string SpecVersionKafkaHeader = KafkaHeaderPrefix + "specversion";
        internal const string ContentType = "content-type";

        public static CloudEvent ToCloudEvent(this ConsumeResult<string?, byte[]> message, CloudEventFormatter cloudEventFormatter, string source, string topicName)
        {

            string contentType = "application/json";
            if (message.Message.Headers.TryGetLastBytes(ContentType, out var b))
            {
                contentType = Encoding.UTF8.GetString(b);
                if (MimeUtilities.IsCloudEventsContentType(contentType))
                {
                    return cloudEventFormatter.DecodeStructuredModeMessage(message.Message.Value, new System.Net.Mime.ContentType(contentType), null);
                }
            }

            var cloudEvent = new CloudEvent();
            if (message.Message.Headers != null && message.Message.Headers.TryGetLastBytes(SpecVersionKafkaHeader, out var bytes))
            {
                var version = Encoding.UTF8.GetString(bytes);
                var specVersion = CloudEventsSpecVersion.FromVersionId(version);
                cloudEvent = new CloudEvent(specVersion);
            }

            cloudEvent.Id = Guid.NewGuid().ToString();
            cloudEvent.Source = new Uri($"{source}:{topicName}");
            cloudEvent.Type = message.Topic;
            cloudEvent.DataContentType = contentType;
            cloudEvent.Data = Encoding.UTF8.GetString(message.Message.Value);
            return cloudEvent;
        }

        public static Message<string?, byte[]> ToKafkaMessage(this CloudEvent cloudEvt, CloudEventFormatter cloudEventFormatter)
        {
            string? key = (string?)cloudEvt[Partitioning.PartitionKeyAttribute];
            var headers = new Headers
            {
                { SpecVersionKafkaHeader, Encoding.UTF8.GetBytes(cloudEvt.SpecVersion.VersionId) }
            };
            foreach (var pair in cloudEvt.GetPopulatedAttributes())
            {
                var attribute = pair.Key;
                if (attribute == cloudEvt.SpecVersion.DataContentTypeAttribute || attribute.Name == Partitioning.PartitionKeyAttribute.Name)
                {
                    continue;
                }

                var v = attribute.Format(pair.Value);
                headers.Add(KafkaHeaderPrefix + attribute.Name, Encoding.UTF8.GetBytes(v));
            }

            byte[] value = BinaryDataUtilities.AsArray(cloudEventFormatter.EncodeBinaryModeEventData(cloudEvt));
            return new Message<string?, byte[]>
            {
                Value = value,
                Headers = headers,
                Key = key
            };
        }
    }
}
