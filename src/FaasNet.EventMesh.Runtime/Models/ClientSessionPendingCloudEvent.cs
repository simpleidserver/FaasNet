using CloudNative.CloudEvents;
using CloudNative.CloudEvents.Core;
using CloudNative.CloudEvents.SystemTextJson;
using System;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class ClientSessionPendingCloudEvent
    {
        public CloudEvent Evt
        {
            get
            {
                return new JsonEventFormatter().DecodeStructuredModeMessage(new ReadOnlyMemory<byte>(EvtPayload), contentType: null, extensionAttributes: null);
            }
            set
            {
                EvtPayload = BinaryDataUtilities.AsArray(new JsonEventFormatter().EncodeStructuredModeMessage(value, out _));
            }
        }
        public byte[] EvtPayload { get; set; }
        public string BrokerName { get; set; }
        public string Topic { get; set; }
    }
}
