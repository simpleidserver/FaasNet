namespace FaasNet.Peer.Client.Messages
{
    public class AddDirectPartitionResult : BasePartitionedRequest
    {
        public override PartitionedCommands Command => PartitionedCommands.ADD_PARTITION_RESULT;

        protected override void SerializeAction(WriteBufferContext context) { }
    }
}
