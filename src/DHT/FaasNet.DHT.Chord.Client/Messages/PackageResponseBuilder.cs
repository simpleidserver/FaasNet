namespace FaasNet.DHT.Chord.Client.Messages
{
    public class PackageResponseBuilder
    {
        public static DHTPackage Create()
        {
            return new DHTPackage(Commands.CREATE_RESULT);
        }

        public static DHTPackage FindSuccessor(string url, int port, long id)
        {
            return new FindSuccessorResult { Id = id, Url = url, Port = port };
        }

        public static DHTPackage Join()
        {
            return new DHTPackage(Commands.JOIN_CHORD_NETWORK_RESULT);
        }

        public static DHTPackage Notify()
        {
            return new DHTPackage(Commands.NOTIFY_RESULT);
        }

        public static DHTPackage NotFoundPredecessor()
        {
            return new FindPredecessorResult { HasPredecessor = false };
        }

        public static DHTPackage FindPredecessor(long id, string url, int port)
        {
            return new FindPredecessorResult { Id = id, Url = url, Port = port, HasPredecessor = true };
        }
    }
}
