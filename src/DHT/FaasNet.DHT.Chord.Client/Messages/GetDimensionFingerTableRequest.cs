namespace FaasNet.DHT.Chord.Client.Messages
{
    public class GetDimensionFingerTableRequest : DHTPackage
    {
        public GetDimensionFingerTableRequest() : base(Commands.GET_DIMENSION_FINGER_TABLE_REQUEST)
        {
        }
    }
}
