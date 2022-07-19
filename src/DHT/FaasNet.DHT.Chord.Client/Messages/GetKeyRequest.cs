using FaasNet.Peer.Client;

namespace FaasNet.DHT.Chord.Client.Messages
{
    public class GetKeyRequest : ChordPackage
    {
        public long Id { get; set; }
        public override ChordCommandTypes Command => ChordCommandTypes.GET_KEY_REQUEST;

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(Id);
        }

        public void Extract(ReadBufferContext context)
        {
            Id = context.NextLong();
        }
    }
}
