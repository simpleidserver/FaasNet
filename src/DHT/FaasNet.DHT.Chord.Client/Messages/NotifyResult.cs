using FaasNet.Peer.Client;

namespace FaasNet.DHT.Chord.Client.Messages
{
    public class NotifyResult : ChordPackage
    {
        public override ChordCommandTypes Command => ChordCommandTypes.NOTIFY_RESULT;

        public override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}
