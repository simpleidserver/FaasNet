using FaasNet.Peer.Client;

namespace FaasNet.DHT.Chord.Client.Messages
{
    public class GetDimensionFingerTableRequest : ChordPackage
    {
        public override ChordCommandTypes Command => ChordCommandTypes.GET_DIMENSION_FINGER_TABLE_REQUEST;

        public override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}
