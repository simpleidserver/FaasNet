using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Net.Mime;

namespace FaasNet.EventMesh.Runtime.MessageBroker
{
    public class EventMeshCloudEvent
    {
        private static IEnumerable<CloudEventAttribute> AllTypesExtensions { get; } = new List<CloudEventAttribute>
        {
            CloudEventAttribute.CreateExtension("binary", CloudEventAttributeType.Binary),
            CloudEventAttribute.CreateExtension("boolean", CloudEventAttributeType.Boolean),
            CloudEventAttribute.CreateExtension("integer", CloudEventAttributeType.Integer),
            CloudEventAttribute.CreateExtension("string", CloudEventAttributeType.String),
            CloudEventAttribute.CreateExtension("timestamp", CloudEventAttributeType.Timestamp),
            CloudEventAttribute.CreateExtension("uri", CloudEventAttributeType.Uri),
            CloudEventAttribute.CreateExtension("urireference", CloudEventAttributeType.UriReference)
        }.AsReadOnly();

        public CloudEvent CloudEvt
        {
            get
            {
                if(CloudEvtPayload == null)
                {
                    return null;
                }

                var formatter = new JsonEventFormatter();
                var contentType = new ContentType("application/cloudevents+json; charset=utf-8");
                var result = formatter.DecodeStructuredModeMessage(CloudEvtPayload, contentType, AllTypesExtensions);
                var data = result.Data.ToString().Trim('"');
                result.Data = data;
                return result;
            }
            set
            {
                if(value == null)
                {
                    CloudEvtPayload = null;
                }

                var encoded = new JsonEventFormatter().EncodeStructuredModeMessage(value, out var contentType);
                CloudEvtPayload = encoded.ToArray();
            }
        }
        public string Topic { get; set; }
        public DateTime CreateDateTime { get; set; }
        public byte[] CloudEvtPayload { get; set; }
    }
}
