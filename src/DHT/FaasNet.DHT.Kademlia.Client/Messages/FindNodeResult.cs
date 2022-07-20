using FaasNet.Peer.Client;
using System.Collections.Generic;

namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class FindNodeResult : KademliaPackage
    {
        public FindNodeResult()
        {
            Peers = new List<PeerResult>();    
        }

        public override KademliaCommandTypes Command => KademliaCommandTypes.FIND_NODE_RESULT;
        public ICollection<PeerResult> Peers { get; set; }

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger(Peers.Count);
            foreach (PeerResult peerResult in Peers) peerResult.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            var length = context.NextInt();
            for (var i = 0; i < length; i++) Peers.Add(PeerResult.Deserialize(context));
        }
    }

    public class PeerResult
    {
        public long Id { get; set; }
        public string Url  { get; set; }
        public int Port { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteLong(Id);
            context.WriteString(Url);
            context.WriteInteger(Port);
        }

        public static PeerResult Deserialize(ReadBufferContext context)
        {
            return new PeerResult { Id = context.NextLong(), Url = context.NextString(), Port = context.NextInt() };
        }
    }
}
