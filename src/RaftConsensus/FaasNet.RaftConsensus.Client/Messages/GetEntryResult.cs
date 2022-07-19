using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class GetEntryResult : BaseConsensusPackage
    {
        public bool IsNotFound { get; set; }
        public string Value { get; set; }

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteBoolean(IsNotFound);
            if(!IsNotFound) context.WriteString(Value);
        }

        public void Extract(ReadBufferContext context)
        {
            IsNotFound = context.NextBoolean();
            if(!IsNotFound) Value = context.NextString();
        }
    }
}
