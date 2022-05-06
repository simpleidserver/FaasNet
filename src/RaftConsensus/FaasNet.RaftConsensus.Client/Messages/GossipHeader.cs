namespace FaasNet.RaftConsensus.Client.Messages
{
    public class GossipHeader
    {
        public GossipHeader(GossipCommands command)
        {
            Command = command;
        }

        public GossipCommands Command { get; private set; }

        public void Serialize(WriteBufferContext context)
        {
            Command.Serialize(context);
        }

        public static GossipHeader Deserialize(ReadBufferContext context)
        {
            var cmd = GossipCommands.Deserialize(context);
            return new GossipHeader(cmd);
        }
    }
}