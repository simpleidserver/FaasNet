using FaasNet.Peer.Client;

namespace FaasNet.Partition.Client.Messages
{
    public class PartitionedCommands : BaseEnumeration
    {
        public static PartitionedCommands TRANSFERED_REQUEST = new PartitionedCommands(0, "TRANSFERED_REQUEST");

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
