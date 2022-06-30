namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class FindValueRequest : BasePackage
    {
        public FindValueRequest() : base(Commands.FIND_VALUE_REQUEST)
        {
        }
    }
}
