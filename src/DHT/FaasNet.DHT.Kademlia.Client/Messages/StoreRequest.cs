namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class StoreRequest : BasePackage
    {
        public StoreRequest() : base(Commands.STORE_REQUEST)
        {
        }

        public long Key { get; set; }
        public string Value { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteLong(Key);
            context.WriteString(Value);
        }

        public override BasePackage Extract(ReadBufferContext context)
        {
            base.Extract(context);
            Key = context.NextLong();
            Value = context.NextString();
            return this;
        }
    }
}
