namespace FaasNet.DHT.Chord.Client.Messages
{
    public class GetKeyResult : DHTPackage
    {
        public GetKeyResult() : base(Commands.GET_KEY_RESULT)
        {

        }

        public long Id { get; set; }
        public string Value { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteLong(Id);
            context.WriteString(Value);
        }

        public void Extract(ReadBufferContext context)
        {
            Id = context.NextLong();
            Value = context.NextString();
        }
    }
}
