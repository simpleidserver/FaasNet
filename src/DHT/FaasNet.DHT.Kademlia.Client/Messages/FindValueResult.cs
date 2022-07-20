using FaasNet.Peer.Client;

namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class FindValueResult : KademliaPackage
    {
        public long Key { get; set; }
        public string Value { get; set; }
        public override KademliaCommandTypes Command => KademliaCommandTypes.FIND_VALUE_RESULT;

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(Key);
            context.WriteString(Value);
        }

        public void Extract(ReadBufferContext context)
        {
            Key = context.NextLong();
            Value = context.NextString();
        }
    }
}
