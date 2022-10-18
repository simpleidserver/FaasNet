using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.Messages
{
    public class UpdateApplicationDomainCoordinatesRequest : BaseEventMeshPackage
    {
        public UpdateApplicationDomainCoordinatesRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.UPDATE_APPLICATION_DOMAIN_COORDINATES_REQUEST;
        public string Name { get; set; }
        public string Vpn { get; set; }
        public ICollection<ApplicationDomainCoordinate> Coordinates { get; set; } = new List<ApplicationDomainCoordinate>();

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Vpn);
            context.WriteInteger(Coordinates.Count);
            foreach (var coordinate in Coordinates) coordinate.Serialize(context);
        }

        public UpdateApplicationDomainCoordinatesRequest Extract(ReadBufferContext context)
        {
            Name = context.NextString();
            Vpn = context.NextString();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++)
            {
                var coordinate = new ApplicationDomainCoordinate();
                coordinate.Deserialize(context);
                Coordinates.Add(coordinate);
            }

            return this;
        }
    }

    public class ApplicationDomainCoordinate : ISerializable
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
