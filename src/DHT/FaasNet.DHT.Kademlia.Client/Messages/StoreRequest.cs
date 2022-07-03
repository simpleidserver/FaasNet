namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class StoreRequest : BasePackage
    {
        public StoreRequest() : base(Commands.STORE_REQUEST)
        {
        }

        public bool Force { get; set; }
        public long Key { get; set; }
        public string Value { get; set; }
        public long ExcludedPeer { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteBoolean(Force);
            context.WriteLong(Key);
            context.WriteString(Value);
            if (Force) context.WriteLong(ExcludedPeer);
        }

        public override BasePackage Extract(ReadBufferContext context)
        {
            base.Extract(context);
            Force = context.NextBoolean();
            Key = context.NextLong();
            Value = context.NextString();
            if(Force) ExcludedPeer = context.NextLong();
            return this;
        }
    }
}
