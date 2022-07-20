using FaasNet.Peer.Client;

namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class FindNodeRequest : KademliaPackage
    {
        public override KademliaCommandTypes Command => KademliaCommandTypes.FIND_NODE_REQUEST;
        public long Id { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
        public long TargetId { get; set; }

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(Id);
            context.WriteString(Url);
            context.WriteInteger(Port);
            context.WriteLong(TargetId);
        }

        public void Extract(ReadBufferContext context)
        {
            Id = context.NextLong();
            Url = context.NextString();
            Port = context.NextInt();
            TargetId = context.NextLong();
        }
    }
}
