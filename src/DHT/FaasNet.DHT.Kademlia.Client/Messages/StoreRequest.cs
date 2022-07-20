using FaasNet.Peer.Client;

namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class StoreRequest : KademliaPackage
    {
        public bool Force { get; set; }
        public long Key { get; set; }
        public string Value { get; set; }
        public long ExcludedPeer { get; set; }
        public override KademliaCommandTypes Command => KademliaCommandTypes.STORE_REQUEST;

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteBoolean(Force);
            context.WriteLong(Key);
            context.WriteString(Value);
            if (Force) context.WriteLong(ExcludedPeer);
        }

        public KademliaPackage Extract(ReadBufferContext context)
        {
            Force = context.NextBoolean();
            Key = context.NextLong();
            Value = context.NextString();
            if(Force) ExcludedPeer = context.NextLong();
            return this;
        }
    }
}
