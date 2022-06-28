namespace FaasNet.DHT.Chord.Client.Messages
{
    public class FindPredecessorRequest : DHTPackage
    {
        public FindPredecessorRequest() : base(Commands.FIND_PREDECESSOR_REQUEST)
        {
        }
    }
}
