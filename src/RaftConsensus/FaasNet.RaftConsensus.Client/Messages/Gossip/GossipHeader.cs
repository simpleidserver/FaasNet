namespace FaasNet.RaftConsensus.Client.Messages.Gossip
{
    public class GossipHeader
    {
        public GossipHeader(GossipCommands command, string sourceUrl, int sourcePort)
        {
            Command = command;
            SourceUrl = sourceUrl;
            SourcePort = sourcePort;
        }

        public GossipCommands Command { get; private set; }
        public string SourceUrl { get; private set; }
        public int SourcePort { get; private set; }

        public void Serialize(WriteBufferContext context)
        {
            Command.Serialize(context);
            context.WriteString(SourceUrl);
            context.WriteInteger(SourcePort);
        }

        public static GossipHeader Deserialize(ReadBufferContext context)
        {
            var cmd = GossipCommands.Deserialize(context);
            return new GossipHeader(cmd, context.NextString(), context.NextInt());
        }
    }
}