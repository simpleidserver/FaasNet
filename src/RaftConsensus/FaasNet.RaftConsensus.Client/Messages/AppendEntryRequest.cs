using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class AppendEntryRequest : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.APPEND_ENTRY_REQUEST;

        public byte[] Payload { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteByteArray(Payload);
        }

        public AppendEntryRequest Extract(ReadBufferContext context)
        {
            Payload = context.NextByteArray();
            return this;
        }
    }
}
