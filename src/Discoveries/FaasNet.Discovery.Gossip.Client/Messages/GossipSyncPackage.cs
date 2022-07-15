using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Discovery.Gossip.Client.Messages
{
    public class GossipSyncPackage : GossipPackage
    {
        public GossipSyncPackage()
        {
            PeerInfos = new List<PeerInfo>();
        }

        public override GossipPackageTypes Type => GossipPackageTypes.SYNC;

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

    public class PeerInfo
    {
        public string Url { get; set; }
        public int Port { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Url);
            context.WriteInteger(Port);
        }

        public static PeerInfo Deserialize(ReadBufferContext context)
        {
            return new PeerInfo
            {
                Url = context.NextString(),
                Port = context.NextInt()
            };
        }
    }
}
