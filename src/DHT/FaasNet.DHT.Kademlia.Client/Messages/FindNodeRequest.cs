namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class FindNodeRequest : BasePackage
    {
        public FindNodeRequest() : base(Commands.FIND_NODE_REQUEST)
        {
        }
    }
}
