using FaasNet.Peer.Client;

namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class PingResult : KademliaPackage
    {
        public override KademliaCommandTypes Command => KademliaCommandTypes.PING_RESULT;

        public override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}
