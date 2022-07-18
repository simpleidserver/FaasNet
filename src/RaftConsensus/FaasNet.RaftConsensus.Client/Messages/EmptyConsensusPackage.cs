using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class EmptyConsensusPackage : BaseConsensusPackage
    {
        public override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}
