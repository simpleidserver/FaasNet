namespace FaasNet.Peer.Client.Messages
{
    public class TransferedRequest : BasePartitionedRequest
    {
        public override PartitionedCommands Command => PartitionedCommands.TRANSFERED_REQUEST;

        public string PartitionKey { get; set; }
        public byte[] Content { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(PartitionKey);
            context.WriteByteArray(Content);
        }

        public TransferedRequest Extract(ReadBufferContext context)
        {
            PartitionKey = context.NextString();
            Content = context.NextByteArray();
            return this;
        }
    }
}
