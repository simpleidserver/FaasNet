namespace FaasNet.RaftConsensus.Client.Messages.Consensus
{
    public class AppendEntryRequest : ConsensusPackage
    {
        public string Value { get; set; }
        public bool IsProxified { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(Value);
            context.WriteBoolean(IsProxified);
        }

        public void Extract(ReadBufferContext context)
        {
            Value = context.NextString();
            IsProxified = context.NextBoolean();
        }
    }
}
