namespace FaasNet.RaftConsensus.Client.Messages
{
    public class Header
    {
        public Header(ConsensusCommands command, string termId, long termIndex)
        {
            Command = command;
            TermId = termId;
            TermIndex = termIndex;
        }

        public ConsensusCommands Command { get; set; }
        public string TermId { get; set; }
        public long TermIndex { get; set; }

        public virtual void Serialize(WriteBufferContext context)
        {
            Command.Serialize(context);
            context.WriteString(TermId);
            context.WriteLong(TermIndex);
        }

        public static Header Deserialize(ReadBufferContext context)
        {
            var cmd = ConsensusCommands.Deserialize(context);
            return new Header(cmd, context.NextString(), context.NextLong());
        }
    }
}
