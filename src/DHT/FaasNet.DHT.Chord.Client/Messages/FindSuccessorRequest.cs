using FaasNet.Peer.Client;

namespace FaasNet.DHT.Chord.Client.Messages
{
    public class FindSuccessorRequest : ChordPackage
    {
        public long NodeId { get; set; }
        public override ChordCommandTypes Command => ChordCommandTypes.FIND_SUCCESSOR_REQUEST;

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(NodeId);
        }

        public void Extract(ReadBufferContext context)
        {
            NodeId = context.NextLong();
        }
    }
}
