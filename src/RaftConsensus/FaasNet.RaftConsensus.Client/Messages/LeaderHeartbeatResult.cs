namespace FaasNet.RaftConsensus.Client.Messages
{
    public class LeaderHeartbeatResult : ConsensusPackage
    {
        public string Url { get; set; }
        public int Port { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(Url);
            context.WriteInteger(Port);
        }

        public void Extract(ReadBufferContext context)
        {
            Url = context.NextString();
            Port = context.NextInt();
        }
    }
}
