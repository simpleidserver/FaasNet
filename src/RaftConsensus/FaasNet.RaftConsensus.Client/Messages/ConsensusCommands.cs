using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class ConsensusCommands : BaseEnumeration
    {
        public static ConsensusCommands LEADER_HEARTBEAT_REQUEST = new ConsensusCommands(0, "LEADER_HEARTBEAT_REQUEST");
        public static ConsensusCommands VOTE_REQUEST = new ConsensusCommands(1, "VOTE_REQUEST");
        public static ConsensusCommands VOTE_RESULT = new ConsensusCommands(2, "VOTE_RESULT");
        public static ConsensusCommands APPEND_ENTRY_REQUEST = new ConsensusCommands(3, "APPEND_ENTRY_REQUEST");
        public static ConsensusCommands LEADER_HEARTBEAT_RESULT = new ConsensusCommands(4, "LEADER_HEARTBEAT_RESULT");
        public static ConsensusCommands EMPTY_RESULT = new ConsensusCommands(5, "EMPTY_RESULT");
        public static ConsensusCommands GET_REQUEST = new ConsensusCommands(6, "GET_REQUEST");
        public static ConsensusCommands GET_RESULT = new ConsensusCommands(7, "GET_RESULT");

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
