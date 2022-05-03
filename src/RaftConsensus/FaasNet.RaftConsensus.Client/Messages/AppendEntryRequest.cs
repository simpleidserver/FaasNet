namespace FaasNet.RaftConsensus.Client.Messages
{
    public class AppendEntryRequest : ConsensusPackage
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsBroadcasted { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(Key);
            context.WriteString(Value);
            context.WriteBoolean(IsBroadcasted);
        }

        public void Extract(ReadBufferContext context)
        {
            Key = context.NextString();
            Value = context.NextString();
            IsBroadcasted = context.NextBoolean();
        }
    }
}
