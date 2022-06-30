namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class BasePackage
    {
        public BasePackage(Commands command)
        {
            Command = command;
        }

        public Commands Command { get; private set; }
    }
}
