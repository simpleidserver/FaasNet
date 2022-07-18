using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class LeaderHeartbeatResult : BaseConsensusPackage
    {
        public override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}
