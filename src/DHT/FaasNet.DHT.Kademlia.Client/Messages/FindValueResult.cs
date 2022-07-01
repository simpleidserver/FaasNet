namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class FindValueResult : BasePackage
    {
        public FindValueResult() : base(Commands.FIND_VALUE_RESULT)
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
