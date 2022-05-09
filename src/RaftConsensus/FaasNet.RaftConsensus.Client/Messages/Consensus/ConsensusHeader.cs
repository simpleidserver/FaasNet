namespace FaasNet.RaftConsensus.Client.Messages.Consensus
{
    public class ConsensusHeader
    {
        public ConsensusHeader(ConsensusCommands command, string termId, long termIndex, string sourceUrl, int sourcePort)
        {
            Command = command;
            TermId = termId;
            TermIndex = termIndex;
            SourceUrl = sourceUrl;
            SourcePort = sourcePort;
        }

        public ConsensusCommands Command { get; set; }
        public string TermId { get; set; }
        public long TermIndex { get; set; }
        public string SourceUrl { get; set; }
        public int SourcePort { get; set; }

        public virtual void Serialize(WriteBufferContext context)
        {
            Command.Serialize(context);
            context.WriteString(TermId);
            context.WriteLong(TermIndex);
            context.WriteString(SourceUrl);
            context.WriteInteger(SourcePort);
        }

        public static ConsensusHeader Deserialize(ReadBufferContext context)
        {
            var cmd = ConsensusCommands.Deserialize(context);
            return new ConsensusHeader(cmd, context.NextString(), context.NextLong(), context.NextString(), context.NextInt());
        }
    }
}
