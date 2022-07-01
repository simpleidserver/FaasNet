namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public static class PackageRequestBuilder
    {
        public static BasePackage Ping(long id, string url, int port, string nonce)
        {
            return new PingRequest { Id = id, Url = url, Port = port, Nonce = nonce};
        }

        public static BasePackage FindNode(long id, string url, int port, long targetId, string nonce)
        {
            return new FindNodeRequest { Id = id, Url = url, Port = port, TargetId = targetId, Nonce = nonce };
        }

        public static BasePackage FindValue(long key, string nonce)
        {
            return new FindValueRequest {  Key = key, Nonce = nonce };
        }

        public static BasePackage StoreValue(long key, string value, string nonce)
        {
            return new StoreRequest { Key = key, Value = value, Nonce = nonce };
        }

        public static BasePackage Join(long id, string url, int port, string nonce)
        {
            return new JoinRequest { Id = id, Url = url, Port = port, Nonce = nonce };
        }
    }
}
