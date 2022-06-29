namespace FaasNet.DHT.Chord.Client.Messages
{
    public class AddKeyRequest : DHTPackage
    {
        public AddKeyRequest() : base(Commands.ADD_KEY_REQUEST)
        {

        }

        public long Id { get; set; }
        public string Value { get; set; }
        public bool Force { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteLong(Id);
            context.WriteString(Value);
            context.WriteBoolean(Force);
        }

        public void Extract(ReadBufferContext context)
        {
            Id = context.NextLong();
            Value = context.NextString();
            Force = context.NextBoolean();
        }
    }
}
