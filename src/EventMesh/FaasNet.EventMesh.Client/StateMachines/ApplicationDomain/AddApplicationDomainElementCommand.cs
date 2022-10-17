using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.ApplicationDomain
{
    public class AddApplicationDomainElementCommand : ICommand
    {
        public string Name { get; set; }
        public string Vpn { get; set; }
        public string ElementId { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Name = context.NextString();
            Vpn = context.NextString();
            ElementId = context.NextString();
            CoordinateX = context.NextDouble();
            CoordinateY = context.NextDouble();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Vpn);
            context.WriteString(ElementId);
            context.WriteDouble(CoordinateX);
            context.WriteDouble(CoordinateY);
        }
    }
}
