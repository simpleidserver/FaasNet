namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public static class PackageRequestBuilder
    {
        public static KademliaPackage Ping(string nonce)
        {
            return new PingRequest { Nonce = nonce};
        }

        public static KademliaPackage FindNode(long id, string url, int port, long targetId, string nonce)
        {
            return new FindNodeRequest { Id = id, Url = url, Port = port, TargetId = targetId, Nonce = nonce };
        }

        public static KademliaPackage FindValue(long key, string nonce)
        {
            return new FindValueRequest {  Key = key, Nonce = nonce };
        }

        public static KademliaPackage StoreValue(long key, string value, string nonce)
        {
            return new StoreRequest { Key = key, Value = value, Nonce = nonce, Force = false };
        }

        public static KademliaPackage ForceStoreValue(long key, string value, string nonce, long excludedPeer)
        {
            return new StoreRequest { Key = key, Value = value, Nonce = nonce, Force = true, ExcludedPeer = excludedPeer };
        }
    }
}
