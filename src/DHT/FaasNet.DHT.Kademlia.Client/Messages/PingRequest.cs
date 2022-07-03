namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class PingRequest : BasePackage
    {
        public PingRequest() : base(Commands.PING_REQUEST)
        {
        }
    }
}
