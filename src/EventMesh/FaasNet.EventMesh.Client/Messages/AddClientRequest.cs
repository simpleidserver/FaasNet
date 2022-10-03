using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.Peer.Client;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddClientRequest : BaseEventMeshPackage
    {
        public AddClientRequest(string seq) : base(seq) 
        {
            Purposes = new List<ClientPurposeTypes>();
        }

        public AddClientRequest(string seq, string clientId, string vpn, ICollection<ClientPurposeTypes> purposes, double coordinateX, double coordinateY) : this(seq)
        {
            Id = clientId;
            Vpn = vpn;
            Purposes = purposes;
            CoordinateX = coordinateX;
            CoordinateY = coordinateY;
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_CLIENT_REQUEST;
        public string Id { get; set; }
        public string Vpn { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public ICollection<ClientPurposeTypes> Purposes { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Vpn);
            context.WriteInteger(Purposes.Count);
            foreach (var purpose in Purposes) context.WriteInteger((int)purpose);
            context.WriteDouble(CoordinateX);
            context.WriteDouble(CoordinateY);
        }

        public AddClientRequest Extract(ReadBufferContext context)
        {
            Id = context.NextString();
            Vpn = context.NextString();
            var nbPurposes = context.NextInt();
            for (var i = 0; i < nbPurposes; i++) Purposes.Add((ClientPurposeTypes)context.NextInt());
            CoordinateX = context.NextDouble();
            CoordinateY = context.NextDouble();
            return this;
        }
    }
}
