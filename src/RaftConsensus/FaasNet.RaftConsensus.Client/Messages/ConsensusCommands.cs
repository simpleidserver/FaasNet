namespace FaasNet.RaftConsensus.Client.Messages
{
    public class ConsensusCommands : BaseCommands
    {
        public static ConsensusCommands LEADER_HEARTBEAT_REQUEST = new ConsensusCommands(0, "LEADER_HEARTBEAT_REQUEST");

        protected ConsensusCommands(int code) : base(code) { }
        protected ConsensusCommands(int code, string name) : base(code, name) { }

        public static ConsensusCommands Deserialize(ReadBufferContext context)
        {
            return new ConsensusCommands(context.NextInt());
        }
    }
}
