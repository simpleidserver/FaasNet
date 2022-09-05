namespace FaasNet.Peer.Client.Messages
{
    public class AddDirectPartitionResult : BasePartitionedRequest
    {
        public override PartitionedCommands Command => PartitionedCommands.ADD_PARTITION_RESULT;
        public AddDirectPartitionStatus Status { get; set; }

        protected override void SerializeAction(WriteBufferContext context) { }
    }

    public enum AddDirectPartitionStatus
    {
        SUCCESS = 0,
        ERROR = 1
    }
}
