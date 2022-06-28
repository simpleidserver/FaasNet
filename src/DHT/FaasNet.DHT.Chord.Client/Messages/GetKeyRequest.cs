namespace FaasNet.DHT.Chord.Client.Messages
{
    public class GetKeyRequest : DHTPackage
    {
        public GetKeyRequest() : base(Commands.GET_KEY_REQUEST)
        {

        }

        public long Id { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteLong(Id);
        }

        public void Extract(ReadBufferContext context)
        {
            Id = context.NextLong();
        }
    }
}
