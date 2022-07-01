namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class PingResult : BasePackage
    {
        public PingResult() : base(Commands.PING_RESULT)
        {
        }
    }
}
