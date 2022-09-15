using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class ConsensusCommands : BaseEnumeration
    {
        public static ConsensusCommands VOTE_REQUEST = new ConsensusCommands(0, "VOTE_REQUEST");
        public static ConsensusCommands VOTE_RESULT = new ConsensusCommands(1, "VOTE_RESULT");
        public static ConsensusCommands APPEND_ENTRIES_REQUEST = new ConsensusCommands(2, "APPEND_ENTRIES_REQUEST");
        public static ConsensusCommands APPEND_ENTRIES_RESULT = new ConsensusCommands(3, "LEADER_HEARTBEAT_RESULT");
        public static ConsensusCommands APPEND_ENTRY_REQUEST = new ConsensusCommands(4, "APPEND_ENTRY_REQUEST");
        public static ConsensusCommands APPEND_ENTRY_RESULT = new ConsensusCommands(5, "APPEND_ENTRY_RESULT");
        public static ConsensusCommands GET_PEER_STATE_REQUEST = new ConsensusCommands(6, "GET_PEER_STATE_REQUEST");
        public static ConsensusCommands GET_PEER_STATE_RESULT = new ConsensusCommands(7, "GET_PEER_STATE_RESULT");
        public static ConsensusCommands GET_LOGS_REQUEST = new ConsensusCommands(8, "GET_LOGS_REQUEST");
        public static ConsensusCommands GET_LOGS_RESULT = new ConsensusCommands(9, "GET_LOGS_RESULT");
        public static ConsensusCommands INSTALL_SNAPSHOT_REQUEST = new ConsensusCommands(10, "INSTALL_SNAPSHOT_REQUEST");
        public static ConsensusCommands INSTALL_SNAPSHOT_RESULT = new ConsensusCommands(11, "INSTALL_SNAPSHOT_RESULT");
        public static ConsensusCommands QUERY_REQUEST = new ConsensusCommands(12, "QUERY_REQUEST");
        public static ConsensusCommands QUERY_RESULT = new ConsensusCommands(13, "QUERY_RESULT");

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
