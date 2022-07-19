namespace FaasNet.DHT.Chord.Client.Messages
{
    public static class PackageRequestBuilder
    {
        public static ChordPackage Create(int dimFingerTable)
        {
            return new CreateRequest { DimFingerTable = dimFingerTable };
        }

        public static ChordPackage GetDimensionFingerTable()
        {
            return new GetDimensionFingerTableRequest();
        }

        public static ChordPackage FindSuccessor(long nodeId)
        {
            return new FindSuccessorRequest { NodeId = nodeId };
        }

        public static ChordPackage FindPredecessor()
        {
            return new FindPredecessorRequest();
        }

        public static ChordPackage Join(string url, int port)
        {
            return new JoinChordNetworkRequest { Url = url, Port = port };
        }

        public static ChordPackage Notify(string url, int port, long id)
        {
            return new NotifyRequest { Url = url, Port = port, Id = id };
        }

        public static ChordPackage GetKey(long key)
        {
            return new GetKeyRequest { Id = key };
        }

        public static ChordPackage AddKey(long key, string value, bool force = false)
        {
            return new AddKeyRequest { Id = key, Value = value, Force = force };
        }
    }
}
