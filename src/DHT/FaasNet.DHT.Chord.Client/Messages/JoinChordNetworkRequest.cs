using FaasNet.Peer.Client;

namespace FaasNet.DHT.Chord.Client.Messages
{
    public class JoinChordNetworkRequest : ChordPackage
    {
        public string Url { get; set; }
        public int Port { get; set; }
        public override ChordCommandTypes Command => ChordCommandTypes.JOIN_CHORD_NETWORK_REQUEST;

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Url);
            context.WriteInteger(Port);
        }

        public void Extract(ReadBufferContext context)
        {
            Url = context.NextString();
            Port = context.NextInt();
        }
    }
}
