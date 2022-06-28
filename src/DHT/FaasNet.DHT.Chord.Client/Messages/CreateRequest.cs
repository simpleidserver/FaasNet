namespace FaasNet.DHT.Chord.Client.Messages
{
    public  class CreateRequest : DHTPackage
    {
        public CreateRequest() : base(Commands.CREATE_REQUEST)
        {

        }

        public int DimFingerTable { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteInteger(DimFingerTable);
        }

        public void Extract(ReadBufferContext context)
        {
            DimFingerTable = context.NextInt();
        }
    }
}
