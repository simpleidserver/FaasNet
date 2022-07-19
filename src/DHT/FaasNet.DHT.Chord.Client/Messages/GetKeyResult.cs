using FaasNet.Peer.Client;

namespace FaasNet.DHT.Chord.Client.Messages
{
    public class GetKeyResult : ChordPackage
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public override ChordCommandTypes Command => ChordCommandTypes.GET_KEY_RESULT;

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(Id);
            context.WriteString(Value);
        }

        public void Extract(ReadBufferContext context)
        {
            Id = context.NextLong();
            Value = context.NextString();
        }
    }
}
