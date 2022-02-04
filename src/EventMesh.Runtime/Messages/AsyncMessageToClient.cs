using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using System.Collections.Generic;
using System.Linq;

namespace EventMesh.Runtime.Messages
{
    public class AsyncMessageToClient : Package
    {
        public AsyncMessageToClient()
        {
            CloudEvents = new List<CloudEvent>();
        }

        public string Topic { get; set; }
        public string BrokerName { get; set; }
        public ICollection<CloudEvent> CloudEvents { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(Topic);
            context.WriteString(BrokerName);
            context.WriteInteger(CloudEvents.Count());
            foreach(var cloudEvt in CloudEvents)
            {
                var formatter = new JsonEventFormatter();
                var binary = formatter.EncodeBinaryModeEventData(cloudEvt).ToArray();
                context.WriteByteArray(binary);
            }
        }

        public void Extract(ReadBufferContext context)
        {
            Topic = context.NextString();
            BrokerName = context.NextString();
            int nbCloudEvents = context.NextInt();
            for (int i = 0; i < nbCloudEvents; i++)
            {
                var cloudEventPayload = context.NextByteArray();
                var formatter = new JsonEventFormatter();
                var cloudEvt = new CloudEvent();
                formatter.DecodeBinaryModeEventData(cloudEventPayload, cloudEvt);
                CloudEvents.Add(cloudEvt);
            }
        }
    }
}
