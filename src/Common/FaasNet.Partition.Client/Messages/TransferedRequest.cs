using FaasNet.Peer.Client;

namespace FaasNet.Partition.Client.Messages
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

        public void Extract(ReadBufferContext context)
        {
            PartitionKey = context.NextString();
            Content = context.NextByteArray();
        }
    }
}
