namespace FaasNet.Peer.Client.Messages
{
    public class PartitionedCommands : BaseEnumeration
    {
        public static PartitionedCommands TRANSFERED_REQUEST = new PartitionedCommands(0, "TRANSFERED_REQUEST");
        public static PartitionedCommands ADD_PARTITION_REQUEST = new PartitionedCommands(1, "ADD_PARTITION_REQUEST");
        public static PartitionedCommands ADD_PARTITION_RESULT = new PartitionedCommands(2, "ADD_PARTITION_RESULT");
        public static PartitionedCommands BROADCAST_REQUEST = new PartitionedCommands(3, "BROADCAST_REQUEST");
        public static PartitionedCommands BROADCAST_RESULT = new PartitionedCommands(4, "BROADCAST_RESULT");
        public static PartitionedCommands REMOVE_PARTITION_REQUEST = new PartitionedCommands(5, "REMOVE_PARTITION_REQUEST");
        public static PartitionedCommands REMOVE_PARTITION_RESULT = new PartitionedCommands(6, "REMOVE_PARTITION_RESULT");

        protected PartitionedCommands(int code)
        {
            Init<PartitionedCommands>(code);
        }

        protected PartitionedCommands(int code, string name) : base(code, name) { }

        public static PartitionedCommands Deserialize(ReadBufferContext context)
        {
            return new PartitionedCommands(context.NextInt());
        }
    }
}
