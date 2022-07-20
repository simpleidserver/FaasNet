using FaasNet.Peer.Client;

namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class PingRequest : KademliaPackage
    {
        public override KademliaCommandTypes Command => KademliaCommandTypes.PING_REQUEST;

        public override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}
