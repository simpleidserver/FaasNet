using FaasNet.Peer.Client;

namespace FaasNet.DHT.Chord.Client.Messages
{
    public  class CreateRequest : ChordPackage
    {
        public int DimFingerTable { get; set; }
        public override ChordCommandTypes Command => ChordCommandTypes.CREATE_REQUEST;

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger(DimFingerTable);
        }

        public void Extract(ReadBufferContext context)
        {
            DimFingerTable = context.NextInt();
        }
    }
}
