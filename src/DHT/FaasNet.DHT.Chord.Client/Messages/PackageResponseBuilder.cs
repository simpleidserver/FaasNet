namespace FaasNet.DHT.Chord.Client.Messages
{
    public class PackageResponseBuilder
    {
        public static ChordPackage Create()
        {
            return new EmptyChordPackage();
        }

        public static ChordPackage FindSuccessor(string url, int port, long id)
        {
            return new FindSuccessorResult { Id = id, Url = url, Port = port };
        }

        public static ChordPackage Join()
        {
            return new EmptyChordPackage();
        }

        public static ChordPackage Notify()
        {
            return new NotifyResult();
        }

        public static ChordPackage NotFoundPredecessor()
        {
            return new FindPredecessorResult { HasPredecessor = false };
        }

        public static ChordPackage FindPredecessor(long id, string url, int port)
        {
            return new FindPredecessorResult { Id = id, Url = url, Port = port, HasPredecessor = true };
        }

        public static ChordPackage GetKey(long id, string value)
        {
            return new GetKeyResult { Id = id, Value = value };
        }

        public static ChordPackage AddKey()
        {
            return new AddKeyResult();
        }
    }
}
