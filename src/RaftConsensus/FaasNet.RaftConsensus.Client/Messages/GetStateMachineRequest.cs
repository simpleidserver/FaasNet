using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class GetStateMachineRequest : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.GET_STATEMACHINE_REQUEST;

        public string StateMachineId { get; set; }

        protected override void SerializeAction(WriteBufferContext context) 
        { 
            context.WriteString(StateMachineId);
        }

        public GetStateMachineRequest Extract(ReadBufferContext context)
        {
            StateMachineId = context.NextString();
            return this;
        }
    }
}
