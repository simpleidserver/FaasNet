using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class ReadStateMachineResult : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.READ_STATEMACHINE_RESULT;

        public byte[] StateMachine { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteByteArray(StateMachine);
        }

        public ReadStateMachineResult Extract(ReadBufferContext context)
        {
            StateMachine = context.NextByteArray();
            return this;
        }
    }
}
