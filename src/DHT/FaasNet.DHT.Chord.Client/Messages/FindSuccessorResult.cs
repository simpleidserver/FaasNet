namespace FaasNet.DHT.Chord.Client.Messages
{
    public class FindSuccessorResult : DHTPackage
    {
        public FindSuccessorResult() : base(Commands.FIND_SUCCESSOR_RESULT)
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
