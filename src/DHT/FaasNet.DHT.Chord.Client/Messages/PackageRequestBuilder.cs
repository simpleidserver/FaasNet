namespace FaasNet.DHT.Chord.Client.Messages
{
    public static class PackageRequestBuilder
    {
        public static DHTPackage Create(int dimFingerTable)
        {
            return new CreateRequest { DimFingerTable = dimFingerTable };
        }

        public static DHTPackage GetDimensionFingerTable()
        {
            return new GetDimensionFingerTableRequest();
        }

        public static DHTPackage FindSuccessor(long nodeId)
        {
            return new FindSuccessorRequest { NodeId = nodeId };
        }

        public static DHTPackage FindPredecessor()
        {
            return new FindPredecessorRequest();
        }

        public static DHTPackage Join(string url, int port)
        {
            return new JoinChordNetworkRequest { Url = url, Port = port };
        }

        public static DHTPackage Notify(string url, int port, long id)
        {
            return new NotifyRequest { Url = url, Port = port, Id = id };
        }
    }
}
