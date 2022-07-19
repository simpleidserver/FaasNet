using FaasNet.Peer.Client;

namespace FaasNet.DHT.Chord.Client.Messages
{
    public class NotifyRequest : ChordPackage
    {
        public string Url { get; set; }
        public int Port { get; set; }
        public long Id { get; set; }
        public override ChordCommandTypes Command => ChordCommandTypes.NOTIFY_REQUEST;

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Url);
            context.WriteInteger(Port);
            context.WriteLong(Id);
        }

        public void Extract(ReadBufferContext context)
        {
            Url = context.NextString();
            Port = context.NextInt();
            Id = context.NextLong();
        }
    }
}
