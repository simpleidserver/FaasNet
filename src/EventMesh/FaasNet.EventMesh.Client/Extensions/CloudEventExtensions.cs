﻿using CloudNative.CloudEvents.SystemTextJson;
using FaasNet.Peer.Client;
using System;
using System.Linq;

namespace CloudNative.CloudEvents
{
    public static class CloudEventExtensions
    {
        public static void Serialize(this CloudEvent cloudEvent, WriteBufferContext bufferContext)
        {
            var formatter = new JsonEventFormatter();
            var binary = formatter.EncodeBinaryModeEventData(cloudEvent).ToArray();
            bufferContext.WriteByteArray(binary);
            var attrs = cloudEvent.GetPopulatedAttributes();
            bufferContext.WriteInteger(attrs.Count());
            foreach (var property in cloudEvent.GetPopulatedAttributes())
            {
                bufferContext.WriteString(property.Key.Name);
                bufferContext.WriteString(property.Key.Type.Name);
                bufferContext.WriteString(property.Value.ToString());
            }
        }

        public static CloudEvent NextCloudEvent(this ReadBufferContext context)
        {
            var cloudEventPayload = context.NextByteArray();
            var cloudEvt = new CloudEvent();
            var formatter = new JsonEventFormatter();
            formatter.DecodeBinaryModeEventData(cloudEventPayload, cloudEvt);
            int nbAttributes = context.NextInt();
            for (int i = 0; i < nbAttributes; i++)
            {
                var attrName = context.NextString();
                var type = context.NextString();
                var attrValue = context.NextString();
                switch (type)
                {
                    case "Timestamp":
                        cloudEvt.Time = DateTime.Parse(attrValue);
                        break;
                    default:
                        cloudEvt.SetAttributeFromString(attrName, attrValue);
                        break;
                }
            }

            return cloudEvt;
        }
    }
}
