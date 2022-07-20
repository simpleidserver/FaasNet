using FaasNet.Peer.Client;

namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class FindValueRequest : KademliaPackage
    {
        public long Key { get; set; }
        public override KademliaCommandTypes Command => KademliaCommandTypes.FIND_VALUE_REQUEST;

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(Key);
        }

        public void Extract(ReadBufferContext context)
        {
            Key = context.NextLong();
        }
    }
}
