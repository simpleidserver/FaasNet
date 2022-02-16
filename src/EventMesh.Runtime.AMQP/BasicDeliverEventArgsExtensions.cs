﻿using CloudNative.CloudEvents;
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
            string source,
            string topicName,
            params CloudEventAttribute[]? extensionAttributes)
        {
            var properties = message.BasicProperties;
            if (HasCloudEventsContentType(message, out var contentType))
            {
                return cloudEventFormatter.DecodeStructuredModeMessage(new MemoryStream(message.Body.ToArray()), new ContentType(contentType), extensionAttributes);
            }

            var cloudEvent = new CloudEvent();
            if (properties.Headers.ContainsKey(SpecVersionAmqpHeader))
            {
                var version = Encoding.ASCII.GetString(properties.Headers[SpecVersionAmqpHeader] as byte[]);
                var specVersion = CloudEventsSpecVersion.FromVersionId(version);
                cloudEvent = new CloudEvent(specVersion);
            }

            cloudEvent.Id = Guid.NewGuid().ToString();
            cloudEvent.Source = new Uri($"{source}:{topicName}");
            cloudEvent.Type = message.RoutingKey;
            cloudEvent.DataContentType = properties.ContentType;
            cloudEvent.Data = Encoding.UTF8.GetString(message.Body.ToArray());
            return cloudEvent;
        }

        private static bool HasCloudEventsContentType(BasicDeliverEventArgs message, out string? contentType)
        {
            contentType = message.BasicProperties.ContentType?.ToString();
            return MimeUtilities.IsCloudEventsContentType(contentType);
        }
    }
}