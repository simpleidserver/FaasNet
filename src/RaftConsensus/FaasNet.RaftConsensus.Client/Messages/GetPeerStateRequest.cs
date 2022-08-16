using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class GetPeerStateRequest : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.GET_PEER_STATE_REQUEST;

        protected override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}
