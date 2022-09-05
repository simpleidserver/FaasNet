namespace FaasNet.Peer.Client.Messages
{
    public class RemoveDirectPartitionResult : BasePartitionedRequest
    {
        public override PartitionedCommands Command => PartitionedCommands.REMOVE_PARTITION_RESULT;

        public RemoveDirectPartitionStatus Status { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
        }

        public RemoveDirectPartitionResult Extract(ReadBufferContext context)
        {
            Status = (RemoveDirectPartitionStatus)context.NextInt();
            return this;
        }
    }

    public enum RemoveDirectPartitionStatus
    {
        SUCCESS = 0,
        UNKNOWN_PARTITION = 1
    }
}
