using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class GetPeerStateResult : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.GET_PEER_STATE_RESULT;

        public long Term { get; set; }
        public string VotedFor { get; set; }
        public long CommitIndex { get; set; }
        public long LastApplied { get; set; }
        public PeerStatus Status { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(Term);
            context.WriteString(VotedFor);
            context.WriteLong(CommitIndex);
            context.WriteLong(LastApplied);
            context.WriteInteger((int)Status);
        }

        public void Extract(ReadBufferContext context)
        {
            Term = context.NextLong();
            VotedFor = context.NextString();
            CommitIndex = context.NextLong();
            LastApplied = context.NextLong();
            Status = (PeerStatus)context.NextInt();
        }
    }
}
