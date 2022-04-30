namespace FaasNet.RaftConsensus.Client.Messages
{
    public class ConsensusCommands : BaseCommands
    {
        public static ConsensusCommands LEADER_HEARTBEAT_REQUEST = new ConsensusCommands(0, "LEADER_HEARTBEAT_REQUEST");

        protected ConsensusCommands(int code, string name) : base(code, name)
        {

        }
    }
}
