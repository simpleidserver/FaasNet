namespace FaasNet.RaftConsensus.Client.Messages
{
    public class ConsensusCommands : BaseCommands
    {
        public static ConsensusCommands LEADER_HEARTBEAT_REQUEST = new ConsensusCommands(0, "LEADER_HEARTBEAT_REQUEST");
        public static ConsensusCommands VOTE_REQUEST = new ConsensusCommands(1, "VOTE_REQUEST");
        public static ConsensusCommands VOTE_RESULT = new ConsensusCommands(2, "VOTE_RESULT");

        protected ConsensusCommands(int code)
        {
            Init<ConsensusCommands>(code);
        }

        protected ConsensusCommands(int code, string name) : base(code, name) { }

        public static ConsensusCommands Deserialize(ReadBufferContext context)
        {
            return new ConsensusCommands(context.NextInt());
        }
    }
}
