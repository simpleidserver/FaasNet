namespace FaasNet.DHT.Chord.Client.Messages
{
    public class GetDimensionFingerTableResult : DHTPackage
    {
        public GetDimensionFingerTableResult() : base(Commands.GET_DIMENSION_FINGER_TABLE_RESULT)
        {
        }

        public GetDimensionFingerTableResult(int dimension) : base(Commands.GET_DIMENSION_FINGER_TABLE_RESULT)
        {
            Dimension = dimension;
        }

        public int Dimension { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteInteger(Dimension);
        }

        public void Extract(ReadBufferContext context)
        {
            Dimension = context.NextInt();
        }
    }
}
