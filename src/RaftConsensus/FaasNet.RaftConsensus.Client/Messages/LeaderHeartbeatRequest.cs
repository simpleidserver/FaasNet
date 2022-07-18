using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class LeaderHeartbeatRequest : BaseConsensusPackage
    {
        public override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}