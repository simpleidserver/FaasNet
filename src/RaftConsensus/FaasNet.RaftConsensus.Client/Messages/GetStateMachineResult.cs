using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class GetStateMachineResult : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.GET_STATEMACHINE_RESULT;

        /// <summary>
        /// Index of the state machine.
        /// </summary>
        public long Index { get; set; }
        /// <summary>
        /// Term of the state machine.
        /// </summary>
        public long Term { get; set; }
        /// <summary>
        /// Payload of the state machine.
        /// </summary>
        public byte[] StateMachine { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(Index);
            context.WriteLong(Term);
            context.WriteByteArray(StateMachine);
        }

        public void Extract(ReadBufferContext context)
        {
            Index = context.NextLong();
            Term = context.NextLong();
            StateMachine = context.NextByteArray();
        }
    }
}
