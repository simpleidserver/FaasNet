using CloudNative.CloudEvents;
using EventMesh.Runtime.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace EventMesh.Runtime.Messages
{
    public class AsyncMessageToServer : Package
    {
        public AsyncMessageToServer()
        {
            CloudEvents = new List<CloudEvent>();
        }

        #region Properties

        public string ClientId { get; set; }
        public string Urn { get; set; }
        public string Topic { get; set; }
        public string BrokerName { get; set; }
        public ICollection<CloudEvent> CloudEvents { get; set; }

        #endregion

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(ClientId);
            context.WriteString(Urn);
            context.WriteString(Topic);
            context.WriteString(BrokerName);
            context.WriteInteger(CloudEvents.Count());
            foreach (var cloudEvt in CloudEvents)
            {
                cloudEvt.Serialize(context);
            }
        }

        public void Extract(ReadBufferContext context)
        {
            ClientId = context.NextString();
            Urn = context.NextString();
            Topic = context.NextString();
            BrokerName = context.NextString();
            int nbCloudEvents = context.NextInt();
            for (int i = 0; i < nbCloudEvents; i++)
            {
                CloudEvents.Add(context.DeserializeCloudEvent());
            }
        }
    }
}
