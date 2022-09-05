namespace FaasNet.Peer.Client.Messages
{
    public class AddDirectPartitionRequest : BasePartitionedRequest
    {
        public string PartitionKey { get; set; }
        public string StateMachineType { get; set; }

        public override PartitionedCommands Command => PartitionedCommands.ADD_PARTITION_REQUEST;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(PartitionKey);
            context.WriteString(StateMachineType);
        }

        public AddDirectPartitionRequest Extract(ReadBufferContext context)
        {
            PartitionKey = context.NextString();
            StateMachineType = context.NextString();
            return this;
        }
    }
}
