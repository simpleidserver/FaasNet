namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class JoinRequest : BasePackage
    {
        public JoinRequest() : base(Commands.JOIN_REQUEST)
        {
        }

        public long Id { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }

        public override BasePackage Extract(ReadBufferContext context)
        {
            base.Extract(context);
            Id = context.NextLong();
            Url = context.NextString();
            Port = context.NextInt();
            return this;
        }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteLong(Id);
            context.WriteString(Url);
            context.WriteInteger(Port);
        }
    }
}
