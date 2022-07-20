using FaasNet.Peer.Client;

namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class StoreResult : KademliaPackage
    {
        public override KademliaCommandTypes Command => KademliaCommandTypes.STORE_RESULT;

        public override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}
