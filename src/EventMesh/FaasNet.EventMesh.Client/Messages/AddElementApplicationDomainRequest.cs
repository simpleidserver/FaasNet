using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddElementApplicationDomainRequest : BaseEventMeshPackage
    {
        public AddElementApplicationDomainRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_ELEMENT_APPLICATION_DOMAIN_REQUEST;
        public string Name { get; set; }
        public string Vpn { get; set; }
        public IEnumerable<ApplicationDomainElementPurposeTypes> PurposeTypes { get; set; } = new List<ApplicationDomainElementPurposeTypes>();
        public string ElementId { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Vpn);
            context.WriteInteger(PurposeTypes.Count());
            foreach (var purposeType in PurposeTypes)
                context.WriteInteger((int)purposeType);
            context.WriteString(ElementId);
            context.WriteDouble(CoordinateX);
            context.WriteDouble(CoordinateY);
        }

        public AddElementApplicationDomainRequest Extract(ReadBufferContext context)
        {
            Name = context.NextString();
            Vpn = context.NextString();
            var nb = context.NextInt();
            var lst = new List<ApplicationDomainElementPurposeTypes>();
            for (var i = 0; i < nb; i++)
                lst.Add((ApplicationDomainElementPurposeTypes)context.NextInt());
            PurposeTypes = lst;
            ElementId = context.NextString();
            CoordinateX = context.NextDouble();
            CoordinateY = context.NextDouble();
            return this;
        }
    }
}
