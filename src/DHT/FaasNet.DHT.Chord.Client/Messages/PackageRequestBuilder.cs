namespace FaasNet.DHT.Chord.Client.Messages
{
    public static class PackageRequestBuilder
    {
        public static DHTPackage GetDimensionFingerTable()
        {
            return new GetDimensionFingerTableRequest();
        }

        public static DHTPackage FindSuccessor(long nodeId)
        {
            return new FindSuccessorRequest { NodeId = nodeId };
        }

        public static DHTPackage Join(string url, int port)
        {
            return new JoinChordNetworkRequest { Url = url, Port = port };
        }
    }
}
