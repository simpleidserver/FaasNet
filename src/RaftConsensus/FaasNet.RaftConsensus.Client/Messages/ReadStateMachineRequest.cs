using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class ReadStateMachineRequest : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.READ_STATEMACHINE_REQUEST;

        public long Offset { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(Offset);
        }

        public ReadStateMachineRequest Extract(ReadBufferContext context)
        {
            Offset = context.NextLong();
            return this;
        }
    }
}
