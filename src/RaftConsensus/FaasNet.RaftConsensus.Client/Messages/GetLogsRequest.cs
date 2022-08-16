using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class GetLogsRequest : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.GET_LOGS_REQUEST;

        public long StartIndex { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(StartIndex);
        }

        public void Extract(ReadBufferContext context)
        {
            StartIndex = context.NextLong();
        }
    }
}
