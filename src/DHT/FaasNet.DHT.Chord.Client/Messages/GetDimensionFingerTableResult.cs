using FaasNet.Peer.Client;

namespace FaasNet.DHT.Chord.Client.Messages
{
    public class GetDimensionFingerTableResult : ChordPackage
    {
        public GetDimensionFingerTableResult()
        {

        }

        public GetDimensionFingerTableResult(int dimension)
        {
            Dimension = dimension;
        }

        public int Dimension { get; set; }
        public override ChordCommandTypes Command => ChordCommandTypes.GET_DIMENSION_FINGER_TABLE_RESULT;

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger(Dimension);
        }

        public void Extract(ReadBufferContext context)
        {
            Dimension = context.NextInt();
        }
    }
}
