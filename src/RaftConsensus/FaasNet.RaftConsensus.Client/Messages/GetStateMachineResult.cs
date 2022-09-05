using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class GetStateMachineResult : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.GET_STATEMACHINE_RESULT;

        /// <summary>
        /// Returns true when the state machine is returned.
        /// </summary>
        public bool Success { get; set; }
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
            context.WriteBoolean(Success);
            context.WriteLong(Index);
            context.WriteLong(Term);
            context.WriteByteArray(StateMachine);
        }

        public void Extract(ReadBufferContext context)
        {
            Success = context.NextBoolean();
            if (!Success) return;
            Index = context.NextLong();
            Term = context.NextLong();
            StateMachine = context.NextByteArray();
        }
    }
}
