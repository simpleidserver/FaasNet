namespace FaasNet.Peer.Client.Messages
{
    public class GetAllNodesRequest : BasePartitionedRequest
    {
        public override PartitionedCommands Command => PartitionedCommands.GET_ALL_NODES_REQUEST;

        protected override void SerializeAction(WriteBufferContext context) { }
    }
}
