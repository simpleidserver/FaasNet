using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetPartitionRequest : BaseEventMeshPackage
    {
        public GetPartitionRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.GET_PARTITION_REQUEST;
        public string Partition { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Partition);
        }

        public GetPartitionRequest Extract(ReadBufferContext context)
        {
            Partition = context.NextString();
            return this;
        }
    }
}
