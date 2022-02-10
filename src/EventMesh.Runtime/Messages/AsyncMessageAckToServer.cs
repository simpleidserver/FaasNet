using System.Collections.Generic;
using System.Linq;

namespace EventMesh.Runtime.Messages
{
    public class AsyncMessageAckToServer : Package
    {
        public AsyncMessageAckToServer()
        {
            BridgeServers = new List<AsyncMessageBridgeServer>();
        }

        #region Properties

        public string ClientId { get; set; }
        public int NbCloudEventsConsumed { get; set; }
        public string Topic { get; set; }
        public string BrokerName { get; set; }
        public ICollection<AsyncMessageBridgeServer> BridgeServers { get; set; }

        #endregion

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(ClientId);
            context.WriteInteger(NbCloudEventsConsumed);
            context.WriteString(Topic);
            context.WriteString(BrokerName);
            context.WriteInteger(BridgeServers.Count());
            foreach (var bridgeServer in BridgeServers)
            {
                bridgeServer.Serialize(context);
            }
        }

        public void Extract(ReadBufferContext context)
        {
            ClientId = context.NextString();
            NbCloudEventsConsumed = context.NextInt();
            Topic = context.NextString();
            BrokerName = context.NextString();
            var numberOfBridgeServers = context.NextInt();
            for (int i = 0; i < numberOfBridgeServers; i++)
            {
                BridgeServers.Add(AsyncMessageBridgeServer.Deserialize(context));
            }
        }
    }
}
