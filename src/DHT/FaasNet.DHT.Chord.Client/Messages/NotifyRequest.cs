namespace FaasNet.DHT.Chord.Client.Messages
{
    public class NotifyRequest : DHTPackage
    {
        public NotifyRequest() : base(Commands.NOTIFY_REQUEST)
        {
        }

        public string Url { get; set; }
        public int Port { get; set; }
        public long Id { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(Url);
            context.WriteInteger(Port);
            context.WriteLong(Id);
        }

        public void Extract(ReadBufferContext context)
        {
            Url = context.NextString();
            Port = context.NextInt();
            Id = context.NextLong();
        }
    }
}
