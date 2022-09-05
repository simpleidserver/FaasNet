using FaasNet.Peer.Client;
using System.Collections.Generic;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class GetAllStateMachinesResult : BaseConsensusPackage
    {
        public GetAllStateMachinesResult()
        {
            States = new List<StateMachineResult>();
        }

        public override ConsensusCommands Command => ConsensusCommands.GET_ALL_STATEMACHINES_RESULT;

        public ICollection<StateMachineResult> States { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger(States.Count);
            foreach (var state in States) state.Serialize(context);
        }

        public GetAllStateMachinesResult Extract(ReadBufferContext context)
        {
            var result = new List<StateMachineResult>();
            var nb = context.NextInt();
            for(var i = 0; i < nb; i++) result.Add(StateMachineResult.Extract(context));
            States = result;
            return this;
        }
    }

    public class StateMachineResult
    {
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

        public void Serialize(WriteBufferContext context)
        {
            context.WriteLong(Index);
            context.WriteLong(Term);
            context.WriteByteArray(StateMachine);
        }

        public static StateMachineResult Extract(ReadBufferContext context)
        {
            return new StateMachineResult
            {
                Index = context.NextLong(),
                Term = context.NextLong(),
                StateMachine = context.NextByteArray()
            };
        }
    }
}
