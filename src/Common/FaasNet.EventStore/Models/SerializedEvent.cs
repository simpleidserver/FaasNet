using System;

namespace FaasNet.EventStore.Models
{
    public class SerializedEvent
    {
        public string Topic { get; set; }
        public string Type { get; set; }
        public byte[] Payload { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}