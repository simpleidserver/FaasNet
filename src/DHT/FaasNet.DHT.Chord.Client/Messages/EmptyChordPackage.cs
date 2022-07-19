using FaasNet.Peer.Client;

namespace FaasNet.DHT.Chord.Client.Messages
{
    public class EmptyChordPackage : ChordPackage
    {
        public override ChordCommandTypes Command => ChordCommandTypes.EMPTY_RESULT;

        public override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}
