namespace FaasNet.DHT.Chord.Client.Messages
{
    public class PackageResponseBuilder
    {
        public static DHTPackage FindSuccessor(string url, int port, long id)
        {
            return new FindSuccessorResult { Id = id, Url = url, Port = port };
        }
    }
}
