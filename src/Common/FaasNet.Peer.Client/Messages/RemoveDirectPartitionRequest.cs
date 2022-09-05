namespace FaasNet.Peer.Client.Messages
{
    public class RemoveDirectPartitionRequest : BasePartitionedRequest
    {
        public override PartitionedCommands Command => PartitionedCommands.REMOVE_PARTITION_REQUEST;

        public string PartitionKey { get; set; }

        public RemoveDirectPartitionRequest Extract(ReadBufferContext context)
        {
            PartitionKey = context.NextString();
            return this;
        }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(PartitionKey);
        }
    }
}
