using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class VoteResult : BaseConsensusPackage
    {
        public bool VoteGranted { get; set; }

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteBoolean(VoteGranted);
        }

        public void Extract(ReadBufferContext context)
        {
            VoteGranted = context.NextBoolean();
        }
    }
}
