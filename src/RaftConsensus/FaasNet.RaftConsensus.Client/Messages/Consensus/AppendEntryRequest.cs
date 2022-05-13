namespace FaasNet.RaftConsensus.Client.Messages.Consensus
{
    public class AppendEntryRequest : ConsensusPackage
    {
        public string Value { get; set; }
        public bool IsProxified { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteBoolean(IsProxified);
            context.WriteString(Value);
        }

        public void Extract(ReadBufferContext context)
        {
            IsProxified = context.NextBoolean();
            Value = context.NextString();
        }
    }
}
