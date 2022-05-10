using System.Collections.Generic;

namespace FaasNet.RaftConsensus.Client.Messages.Gossip
{
    public class GossipUpdateClusterRequest : GossipPackage
    {
        public GossipUpdateClusterRequest()
        {
            Nodes = new List<ClusterNodeMessage>();
        }

        public ICollection<ClusterNodeMessage> Nodes { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteInteger(Nodes.Count);
            foreach (var node in Nodes) node.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            var size = context.NextInt();
            for(int i = 0; i < size; i++) Nodes.Add(ClusterNodeMessage.Deserialize(context));
        }
    }

    public class ClusterNodeMessage
    {
        public string Url { get; set; }
        public int Port { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Url);
            context.WriteInteger(Port);
        }

        public static ClusterNodeMessage Deserialize(ReadBufferContext context)
        {
            return new ClusterNodeMessage
            {
                Url = context.NextString(),
                Port = context.NextInt()
            };
        }
    }
}
