namespace FaasNet.RaftConsensus.Client.Messages
{
    public class GossipCommands: BaseCommands
    {
        public static GossipCommands HEARTBEAT_REQUEST = new GossipCommands(0, "HEARTBEAT_REQUEST");
        public static GossipCommands HEARTBEAT_RESULT = new GossipCommands(1, "HEARTBEAT_RESULT");

        protected GossipCommands(int code)
        {
            Init<GossipCommands>(code);
        }

        protected GossipCommands(int code, string name) : base(code, name) { }

        public static GossipCommands Deserialize(ReadBufferContext context)
        {
            return new GossipCommands(context.NextInt());
        }
    }
}
