using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.StateMachines.ApplicationDomain
{
    public class UpdateApplicationDomainCoordinatesCommand : ICommand
    {
        public string Name { get; set; }
        public string Vpn { get; set; }
        public ICollection<UpdateApplicationDomainElement> Elements { get; set; } = new List<UpdateApplicationDomainElement>();

        public void Deserialize(ReadBufferContext context)
        {
            Name = context.NextString();
            Vpn = context.NextString();
            var nb = context.NextInt();
            for(var i = 0; i < nb; i++)
            {
                var elt = new UpdateApplicationDomainElement();
                elt.Deserialize(context);
                Elements.Add(elt);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Vpn);
            context.WriteInteger(Elements.Count());
            foreach (var elt in Elements) elt.Serialize(context);
        }
    }

    public class UpdateApplicationDomainElement : ISerializable
    {
        public string ElementId { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            ElementId = context.NextString();
            CoordinateX = context.NextDouble();
            CoordinateY = context.NextDouble();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(ElementId);
            context.WriteDouble(CoordinateX);
            context.WriteDouble(CoordinateY);
        }
    }
}
