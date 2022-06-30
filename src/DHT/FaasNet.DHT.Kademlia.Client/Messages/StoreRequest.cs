namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class StoreRequest : BasePackage
    {
        public StoreRequest() : base(Commands.STORE_REQUEST)
        {
        }
    }
}
