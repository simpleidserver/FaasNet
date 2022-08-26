using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class GetStateMachineRequest : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.GET_STATEMACHINE_REQUEST;

        protected override void SerializeAction(WriteBufferContext context) { }

        public void Extract(ReadBufferContext context) { }
    }
}
