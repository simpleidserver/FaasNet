using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Discovery.Gossip.Client.Messages
{
    public class GossipGetResultPackage : GossipPackage
    {
        public GossipGetResultPackage()
        {
            PeerInfos = new List<PeerInfo>();
        }

        public override GossipPackageTypes Type => GossipPackageTypes.GETRESULT;

        public ICollection<PeerInfo> PeerInfos { get; set; }

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger(PeerInfos.Count());
            foreach (var peerInfo in PeerInfos) peerInfo.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            var result = new List<PeerInfo>();
            int nb = context.NextInt();
            for (var i = 0; i < nb; i++) result.Add(PeerInfo.Deserialize(context));
            PeerInfos = result;
        }
    }
}
