using FaasNet.Peer.Client;

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
        public string ElementId { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Vpn);
            context.WriteString(ElementId);
            context.WriteDouble(CoordinateX);
            context.WriteDouble(CoordinateY);
        }

        public AddElementApplicationDomainRequest Extract(ReadBufferContext context)
        {
            Name = context.NextString();
            Vpn = context.NextString();
            ElementId = context.NextString();
            CoordinateX = context.NextDouble();
            CoordinateY = context.NextDouble();
            return this;
        }
    }
}
