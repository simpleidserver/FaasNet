using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class GetAllStateMachinesRequest : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.GET_ALL_STATEMACHINES_REQUEST;

        protected override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}
