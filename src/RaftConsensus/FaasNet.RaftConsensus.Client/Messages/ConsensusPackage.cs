namespace FaasNet.RaftConsensus.Client.Messages
{
    public class ConsensusPackage
    {
        private const string MAGIC_CODE = "RaftConsensus";
        private const string PROTOCOL_VERSION = "0000";

        protected ConsensusPackage() { }

        public Header Header { get; private set; }

        public static ConsensusPackage Deserialize(ReadBufferContext context)
        {
            context.NextString();
            context.NextString();
            return null;
        }
    }
}
