namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class StoreResult : BasePackage
    {
        public StoreResult() : base(Commands.STORE_RESULT)
        {
        }
    }
}
