using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class AppendEntryRequest : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.APPEND_ENTRY_REQUEST;

        public string StateMachineId { get; set; }
        public byte[] Payload { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(StateMachineId);
            context.WriteByteArray(Payload);
        }

        public void Extract(ReadBufferContext context)
        {
            StateMachineId = context.NextString();
            Payload = context.NextByteArray();
        }
    }
}
