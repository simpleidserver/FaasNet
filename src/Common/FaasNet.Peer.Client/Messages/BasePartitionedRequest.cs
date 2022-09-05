namespace FaasNet.Peer.Client.Messages
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
                var magicCode = context.NextString();
                var version = context.NextString();
                if (magicCode != MAGIC_CODE || version != PROTOCOL_VERSION) return null;
            }

            var cmd = PartitionedCommands.Deserialize(context);
            if (cmd == PartitionedCommands.TRANSFERED_REQUEST) return new TransferedRequest().Extract(context);
            if (cmd == PartitionedCommands.ADD_PARTITION_REQUEST) return new AddDirectPartitionRequest().Extract(context);
            if (cmd == PartitionedCommands.ADD_PARTITION_RESULT) return new AddDirectPartitionResult();
            if (cmd == PartitionedCommands.BROADCAST_REQUEST) return new BroadcastRequest().Extract(context);
            if (cmd == PartitionedCommands.BROADCAST_RESULT) return new BroadcastResult().Extract(context);
            if (cmd == PartitionedCommands.REMOVE_PARTITION_REQUEST) return new RemoveDirectPartitionRequest().Extract(context);
            if (cmd == PartitionedCommands.REMOVE_PARTITION_RESULT) return new RemoveDirectPartitionResult().Extract(context);
            return null;
        }
    }
}
