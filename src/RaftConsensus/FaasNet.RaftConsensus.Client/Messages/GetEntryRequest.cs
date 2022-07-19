using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class GetEntryRequest : BaseConsensusPackage
    {
        public override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}
