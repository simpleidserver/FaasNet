using System.Collections.Generic;
using System.Linq;

namespace FaasNet.RaftConsensus.Client.Messages.Gossip
{
    public class GossipGetClusterNodesResult : GossipPackage
    {
        public GossipGetClusterNodesResult()
        {
            ClusterNodes = new List<ClusterNodeResult>();
        }

        public ICollection<ClusterNodeResult> ClusterNodes { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            var length = ClusterNodes.Count();
            context.WriteInteger(length);
            foreach (var clusterNode in ClusterNodes) clusterNode.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            var length = context.NextInt();
            for (var i = 0; i < length; i++) ClusterNodes.Add(ClusterNodeResult.Extract(context));
        }
    }

    public class ClusterNodeResult
    {
        public string Url { get; set; }
        public int Port { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Url);
            context.WriteInteger(Port);
        }

        public static ClusterNodeResult Extract(ReadBufferContext context)
        {
            return new ClusterNodeResult { Url = context.NextString(), Port = context.NextInt() };
        }
    }
}
