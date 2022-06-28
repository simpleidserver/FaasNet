namespace FaasNet.DHT.Chord.Client.Messages
{
    public class FindPredecessorResult : DHTPackage
    {
        public FindPredecessorResult() : base(Commands.FIND_PREDECESSOR_RESULT)
        {
        }

        public bool HasPredecessor { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
        public long Id { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteBoolean(HasPredecessor);
            if (!HasPredecessor) return;
            context.WriteString(Url);
            context.WriteInteger(Port);
            context.WriteLong(Id);
        }

        public void Extract(ReadBufferContext context)
        {
            HasPredecessor = context.NextBoolean();
            if (!HasPredecessor) return;
            Url = context.NextString();
            Port = context.NextInt();
            Id = context.NextLong();
        }
    }
}
