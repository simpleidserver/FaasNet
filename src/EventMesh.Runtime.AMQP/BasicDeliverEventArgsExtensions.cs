using CloudNative.CloudEvents;
using CloudNative.CloudEvents.Core;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Net.Mime;
using System.Text;

namespace EventMesh.Runtime.AMQP
{
    public static class BasicDeliverEventArgsExtensions
    {
        internal const string AmqpHeaderPrefix = "cloudEvents:";
        internal const string SpecVersionAmqpHeader = AmqpHeaderPrefix + "specversion";

        public static CloudEvent ToCloudEvent(this BasicDeliverEventArgs message,
            CloudEventFormatter cloudEventFormatter,
            params CloudEventAttribute[]? extensionAttributes)
        {
            var properties = message.BasicProperties;
            if (HasCloudEventsContentType(message, out var contentType))
            {
                return cloudEventFormatter.DecodeStructuredModeMessage(new MemoryStream(message.Body.ToArray()), new ContentType(contentType), extensionAttributes);
            }

            CloudEventsSpecVersion specVersion = null;
            if (properties.Headers.ContainsKey(SpecVersionAmqpHeader))
            {
                var version = Encoding.ASCII.GetString(properties.Headers[SpecVersionAmqpHeader] as byte[]);
                specVersion = CloudEventsSpecVersion.FromVersionId(version);
            }

            var cloudEvent = new CloudEvent
            {
                Id = Guid.NewGuid().ToString(),
                Source = new System.Uri("urn:example-com"),
                Type = message.RoutingKey,
                DataContentType = properties.ContentType,
                Data = Encoding.UTF8.GetString(message.Body.ToArray())
            };
            return cloudEvent;
        }

        private static bool HasCloudEventsContentType(BasicDeliverEventArgs message, out string? contentType)
        {
            contentType = message.BasicProperties.ContentType?.ToString();
            return MimeUtilities.IsCloudEventsContentType(contentType);
        }
    }
}
