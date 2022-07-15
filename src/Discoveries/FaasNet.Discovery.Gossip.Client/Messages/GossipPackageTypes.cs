using FaasNet.Peer.Client;

namespace FaasNet.Discovery.Gossip.Client.Messages
{
    public class GossipPackageTypes : BaseEnumeration
    {
        public static GossipPackageTypes SYNC = new GossipPackageTypes(0, "SYNC");
        public static GossipPackageTypes GET = new GossipPackageTypes(1, "GET");
        public static GossipPackageTypes GETRESULT = new GossipPackageTypes(2, "GETRESULT");
        public static GossipPackageTypes RESULT = new GossipPackageTypes(3, "RESULT");

        public GossipPackageTypes(int code)
        {
            Init<GossipPackageTypes>(code);
        }

        public GossipPackageTypes(int code, string name) : base(code, name)
        {
        }

        public static GossipPackageTypes Deserialize(ReadBufferContext context)
        {
            return new GossipPackageTypes(context.NextInt());
        }
    }
}
