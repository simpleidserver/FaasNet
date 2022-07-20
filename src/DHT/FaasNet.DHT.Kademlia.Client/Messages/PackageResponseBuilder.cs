using System.Collections.Generic;

namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public static class PackageResponseBuilder
    {
        public static KademliaPackage Pong(string nonce)
        {
            return new PingResult { Nonce = nonce };
        }

        public static KademliaPackage FindNode(ICollection<PeerResult> peers, string nonce)
        {
            return new FindNodeResult { Nonce = nonce, Peers = peers };
        }

        public static KademliaPackage FindValue(long key, string value, string nonce)
        {
            return new FindValueResult { Key = key, Value = value, Nonce = nonce };
        }

        public static KademliaPackage StoreValue(string nonce)
        {
            return new StoreResult { Nonce = nonce };
        }
    }
}
