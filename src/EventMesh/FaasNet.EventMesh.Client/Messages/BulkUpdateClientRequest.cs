using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.Messages
{
    public class BulkUpdateClientRequest : BaseEventMeshPackage
    {
        public BulkUpdateClientRequest(string seq) : base(seq) 
        {
        }

        public BulkUpdateClientRequest(string seq, string vpn, ICollection<UpdateClientRequest> clients) : this(seq)
        {
            Vpn = vpn;
            Clients = clients;
        }

        public override EventMeshCommands Command => EventMeshCommands.BULK_UPDATE_CLIENT_REQUEST;
        public string Vpn { get; set; }
        public ICollection<UpdateClientRequest> Clients { get; set; } = new List<UpdateClientRequest>();

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteInteger(Clients.Count);
            foreach (var client in Clients) client.Serialize(context);
        }

        public BulkUpdateClientRequest Extract(ReadBufferContext context)
        {
            Vpn = context.NextString();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++)
            {
                var record = new UpdateClientRequest();
                record.Deserialize(context);
                Clients.Add(record);
            }

            return this;
        }
    }

    public class UpdateClientRequest
    {
        public string Id { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public ICollection<string> Targets { get; set; } = new List<string>();

        public UpdateClientRequest()
        {

        }

        public UpdateClientRequest(string clientId, double coordinateX, double coordinateY, ICollection<string> targets)
        {
            Id = clientId;
            CoordinateX = coordinateX;
            CoordinateY = coordinateY;
            Targets = targets;
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteDouble(CoordinateX);
            context.WriteDouble(CoordinateY);
            context.WriteInteger(Targets.Count());
            foreach (var target in Targets) context.WriteString(target);
        }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            CoordinateX = context.NextDouble();
            CoordinateY = context.NextDouble();
            var nbTargets = context.NextInt();
            for (var i = 0; i < nbTargets; i++) Targets.Add(context.NextString());
        }
    }
}
