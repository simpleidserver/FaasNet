using CloudNative.CloudEvents;
using EventMesh.Runtime.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace EventMesh.Runtime.Messages
{
    public class AsyncMessageToClient : Package
    {
        public AsyncMessageToClient()
        {
            BridgeServers = new List<AsyncMessageBridgeServer>();
            CloudEvents = new List<CloudEvent>();
        }

        #region Properties

        public ICollection<AsyncMessageBridgeServer> BridgeServers { get; set; }
        public string Topic { get; set; }
        public string BrokerName { get; set; }
        public ICollection<CloudEvent> CloudEvents { get; set; }

        #endregion

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteInteger(BridgeServers.Count());
            foreach (var bridgeServer in BridgeServers)
            {
                bridgeServer.Serialize(context);
            }

            context.WriteString(Topic);
            context.WriteString(BrokerName);
            context.WriteInteger(CloudEvents.Count());
            foreach(var cloudEvt in CloudEvents)
            {
                cloudEvt.Serialize(context);
            }
        }

        public void Extract(ReadBufferContext context)
        {
            var numberOfBridgeServers = context.NextInt();
            for (int i = 0; i < numberOfBridgeServers; i++)
            {
                BridgeServers.Add(AsyncMessageBridgeServer.Deserialize(context));
            }

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
