using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class ConsensusCommands : BaseEnumeration
    {
        public static ConsensusCommands VOTE_REQUEST = new ConsensusCommands(0, "VOTE_REQUEST");
        public static ConsensusCommands VOTE_RESULT = new ConsensusCommands(1, "VOTE_RESULT");
        public static ConsensusCommands APPEND_ENTRIES_REQUEST = new ConsensusCommands(2, "APPEND_ENTRIES_REQUEST");
        public static ConsensusCommands APPEND_ENTRIES_RESULT = new ConsensusCommands(3, "LEADER_HEARTBEAT_RESULT");

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
