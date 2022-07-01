namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class FindNodeRequest : BasePackage
    {
        public FindNodeRequest() : base(Commands.FIND_NODE_REQUEST)
        {
        }

        public long Id { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
        public long TargetId { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteLong(Id);
            context.WriteString(Url);
            context.WriteInteger(Port);
            context.WriteLong(TargetId);
        }

        public override BasePackage Extract(ReadBufferContext context)
        {
            base.Extract(context);
            Id = context.NextLong();
            Url = context.NextString();
            Port = context.NextInt();
            TargetId = context.NextLong();
            return this;
        }
    }
}
