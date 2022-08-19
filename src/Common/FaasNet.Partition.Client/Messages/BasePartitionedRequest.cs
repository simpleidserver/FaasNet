using FaasNet.Peer.Client;

namespace FaasNet.Partition.Client.Messages
{
    public abstract class BasePartitionedRequest : BasePeerPackage
    {
        public const string MAGIC_CODE = "PARTITIONED";
        public const string PROTOCOL_VERSION = "0000";

        public override string MagicCode => MAGIC_CODE;
        public override string VersionNumber => PROTOCOL_VERSION;
        public abstract PartitionedCommands Command { get; }

        public override void SerializeBody(WriteBufferContext context)
        {
            Command.Serialize(context);
            SerializeAction(context);
        }

        protected abstract void SerializeAction(WriteBufferContext context);

        public static BasePartitionedRequest Deserialize(ReadBufferContext context, bool ignoreEnvelope = false)
        {
            if (!ignoreEnvelope)
            {
                context.NextString();
                context.NextString();
            }

            var cmd = PartitionedCommands.Deserialize(context);
            if (cmd == PartitionedCommands.TRANSFERED_REQUEST)
            {
                var result = new TransferedRequest();
                result.Extract(context);
                return result;
            }

            return null;
        }
    }
}
