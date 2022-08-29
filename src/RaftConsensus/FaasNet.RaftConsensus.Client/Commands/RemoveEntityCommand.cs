using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Commands
{
    public class RemoveEntityCommand : IArrayCommand
    {
        public string Id { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
        }
    }
}
