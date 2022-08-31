using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client.Commands;

namespace FaasNet.EventMesh.Client.StateMachines
{
    public class VpnRecord : IEntityRecord
    {
        public string Id { get; set; }
        public string Description { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Description = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Description);
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
