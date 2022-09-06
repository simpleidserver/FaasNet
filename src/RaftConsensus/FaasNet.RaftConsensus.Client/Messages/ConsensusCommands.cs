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
        public static ConsensusCommands GET_STATEMACHINE_REQUEST = new ConsensusCommands(12, "GET_STATEMACHINE_REQUEST");
        public static ConsensusCommands GET_STATEMACHINE_RESULT = new ConsensusCommands(13, "GET_STATEMACHINE_RESULT");
        public static ConsensusCommands GET_ALL_STATEMACHINES_REQUEST = new ConsensusCommands(14, "GET_ALL_STATEMACHINES_REQUEST");
        public static ConsensusCommands GET_ALL_STATEMACHINES_RESULT = new ConsensusCommands(15, "GET_ALL_STATEMACHINES_RESULT");
        public static ConsensusCommands READ_STATEMACHINE_REQUEST = new ConsensusCommands(16, "READ_STATEMACHINE_REQUEST");
        public static ConsensusCommands READ_STATEMACHINE_RESULT = new ConsensusCommands(17, "READ_STATEMACHINE_RESULT");

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
