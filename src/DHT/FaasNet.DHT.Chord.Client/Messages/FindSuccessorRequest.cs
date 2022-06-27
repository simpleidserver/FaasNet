namespace FaasNet.DHT.Chord.Client.Messages
{
    public class FindSuccessorRequest : DHTPackage
    {
        public FindSuccessorRequest() : base(Commands.FIND_SUCCESSOR_REQUEST)
        {
        }

        public long NodeId { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteLong(NodeId);
        }

        public void Extract(ReadBufferContext context)
        {
            NodeId = context.NextLong();
        }
    }
}
