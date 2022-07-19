using FaasNet.Peer.Client;

namespace FaasNet.DHT.Chord.Client.Messages
{
    public class AddKeyRequest : ChordPackage
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public bool Force { get; set; }
        public override ChordCommandTypes Command => ChordCommandTypes.ADD_KEY_REQUEST;

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(Id);
            context.WriteString(Value);
            context.WriteBoolean(Force);
        }

        public void Extract(ReadBufferContext context)
        {
            Id = context.NextLong();
            Value = context.NextString();
            Force = context.NextBoolean();
        }
    }
}
