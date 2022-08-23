using FaasNet.Peer.Client;

namespace FaasNet.Discovery.Gossip.Client.Messages
{
    public abstract class GossipPackage : BasePeerPackage
    {
        public static string MAGIC_CODE = "GOSSIP";
        public override string MagicCode => MAGIC_CODE;
        public override string VersionNumber => "0000";
        public abstract GossipPackageTypes Type { get; }

        public override void SerializeBody(WriteBufferContext context)
        {
            Type.Serialize(context);
            SerializeAction(context);
        }

        public abstract void SerializeAction(WriteBufferContext context);

        public static GossipPackage Deserialize(ReadBufferContext context, bool ignoreEnvelope = false)
        {
            if (ignoreEnvelope)
            {
                context.NextString();
                context.NextString();
            }

            var type = GossipPackageTypes.Deserialize(context);
            if (type == GossipPackageTypes.RESULT) return new GossipResultPackage();
            if (type == GossipPackageTypes.GET)
            {
                var result = new GossipGetPackage();
                result.Extract(context);
                return result;
            }

            if (type == GossipPackageTypes.GETRESULT)
            {
                var result = new GossipGetResultPackage();
                result.Extract(context);
                return result;
            }

            if (type == GossipPackageTypes.SYNC)
            {
                var result = new GossipSyncPackage();
                result.Extract(context);
                return result;
            }

            return null;
        }
    }
}
