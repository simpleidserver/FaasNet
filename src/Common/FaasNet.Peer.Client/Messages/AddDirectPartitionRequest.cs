namespace FaasNet.Peer.Client.Messages
{
    public class AddDirectPartitionRequest : BasePartitionedRequest
    {
        public string PartitionKey { get; set; }

        public override PartitionedCommands Command => PartitionedCommands.ADD_PARTITION_REQUEST;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(PartitionKey);
        }

        public void Extract(ReadBufferContext context)
        {
            PartitionKey = context.NextString();
        }
    }
}
