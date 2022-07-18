using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class AppendEntryRequest : BaseConsensusPackage
    {
        public string Value { get; set; }
        public bool IsProxified { get; set; }

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteBoolean(IsProxified);
            context.WriteString(Value);
        }

        public void Extract(ReadBufferContext context)
        {
            IsProxified = context.NextBoolean();
            Value = context.NextString();
        }
    }
}
