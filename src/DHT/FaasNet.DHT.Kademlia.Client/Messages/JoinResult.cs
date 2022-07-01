namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class JoinResult : BasePackage
    {
        public JoinResult() : base(Commands.JOIN_RESULT)
        {
        }
    }
}
