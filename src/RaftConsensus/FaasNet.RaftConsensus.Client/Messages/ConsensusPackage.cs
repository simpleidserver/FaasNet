namespace FaasNet.RaftConsensus.Client.Messages
{
    public class ConsensusPackage
    {
        private const string MAGIC_CODE = "RaftConsensus";
        private const string PROTOCOL_VERSION = "0000";

        protected ConsensusPackage() { }

        public Header Header { get; set; }

        public virtual void Serialize(WriteBufferContext context)
        {
            context.WriteString(MAGIC_CODE);
            context.WriteString(PROTOCOL_VERSION);
            Header.Serialize(context);
        }

        public static ConsensusPackage Deserialize(ReadBufferContext context)
        {
            var magicCode = context.NextString();
            var version = context.NextString();
            if (magicCode == MAGIC_CODE || version == PROTOCOL_VERSION) return null;
            var header = Header.Deserialize(context);
            return new ConsensusPackage
            {
                Header = header
            };
        }
    }
}
