using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AsyncMessageToServer : Package
    {
        public AsyncMessageToServer()
        {
            BridgeServers = new List<AsyncMessageBridgeServer>();
            CloudEvents = new List<CloudEvent>();
        }

        #region Properties

        public string ClientId { get; set; }
        public ICollection<AsyncMessageBridgeServer> BridgeServers { get; set; }
        public string TopicMessage { get; set; }
        public string TopicFilter { get; set; }
        public string BrokerName { get; set; }
        public string SessionId { get; set; }
        public ICollection<CloudEvent> CloudEvents { get; set; }

        #endregion

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(ClientId);
            context.WriteInteger(BridgeServers.Count());
            foreach(var bridgeServer in BridgeServers)
            {
                bridgeServer.Serialize(context);
            }

            context.WriteString(TopicMessage);
            context.WriteString(TopicFilter);
            context.WriteString(BrokerName);
            context.WriteString(SessionId);
            context.WriteInteger(CloudEvents.Count());
            foreach (var cloudEvt in CloudEvents)
            {
                cloudEvt.Serialize(context);
            }
        }

        public void Extract(ReadBufferContext context)
        {
            ClientId = context.NextString();
            var numberOfBridgeServers = context.NextInt();
            for(int i = 0; i < numberOfBridgeServers; i++)
            {
                BridgeServers.Add(AsyncMessageBridgeServer.Deserialize(context));
            }

            TopicMessage = context.NextString();
            TopicFilter = context.NextString();
            BrokerName = context.NextString();
            SessionId = context.NextString();
            int nbCloudEvents = context.NextInt();
            for (int i = 0; i < nbCloudEvents; i++)
            {
                CloudEvents.Add(context.DeserializeCloudEvent());
            }
        }
    }
}
