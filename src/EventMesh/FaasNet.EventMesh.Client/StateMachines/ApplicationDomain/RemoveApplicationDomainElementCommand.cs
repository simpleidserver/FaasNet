using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.ApplicationDomain
{
    public class RemoveApplicationDomainElementCommand : ICommand
    {
        public string Name { get; set; }
        public string Vpn { get; set; }
        public string ElementId { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Name = context.NextString();
            Vpn = context.NextString();
            ElementId = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Vpn);
            context.WriteString(ElementId);
        }
    }
}
