using FaasNet.Peer.Client;

namespace FaasNet.DHT.Chord.Client.Messages
{
    public class AddKeyResult : ChordPackage
    {
        public override ChordCommandTypes Command => ChordCommandTypes.ADD_KEY_RESULT;

        public override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}
