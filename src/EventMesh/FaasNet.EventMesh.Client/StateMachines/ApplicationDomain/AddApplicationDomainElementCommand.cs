using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.StateMachines.ApplicationDomain
{
    public class AddApplicationDomainElementCommand : ICommand
    {
        public string Name { get; set; }
        public string Vpn { get; set; }
        public string ElementId { get; set; }
        public IEnumerable<ApplicationDomainElementPurposeTypes> PurposeTypes { get; set; } = new List<ApplicationDomainElementPurposeTypes>();
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Name = context.NextString();
            Vpn = context.NextString();
            ElementId = context.NextString();
            var lst = new List<ApplicationDomainElementPurposeTypes>();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++)
                lst.Add((ApplicationDomainElementPurposeTypes)context.NextInt());
            PurposeTypes = lst;
            CoordinateX = context.NextDouble();
            CoordinateY = context.NextDouble();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Vpn);
            context.WriteString(ElementId);
            context.WriteInteger(PurposeTypes.Count());
            foreach (var purposeType in PurposeTypes)
                context.WriteInteger((int)purposeType);
            context.WriteDouble(CoordinateX);
            context.WriteDouble(CoordinateY);
        }
    }
}
