using System.Collections.Generic;

namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public static class PackageResponseBuilder
    {
        public static BasePackage Pong(string nonce)
        {
            return new PingResult { Nonce = nonce };
        }

        public static BasePackage FindNode(ICollection<PeerResult> peers, string nonce)
        {
            return new FindNodeResult { Nonce = nonce, Peers = peers };
        }

        public static BasePackage FindValue(long key, string value, string nonce)
        {
            return new FindValueResult { Key = key, Value = value, Nonce = nonce };
        }

        public static BasePackage StoreValue(string nonce)
        {
            return new StoreResult { Nonce = nonce };
        }
    }
}
