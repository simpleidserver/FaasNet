using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.StateMachines.Client
{
    public class GetAllClientsQuery : IQuery
    {
        public void Deserialize(ReadBufferContext context) { }

        public void Serialize(WriteBufferContext context) { }
    }

    public class GetAllClientsQueryResult : IQueryResult
    {
        public ICollection<ClientQueryResult> Clients { get; set; } = new List<ClientQueryResult>();

        public void Deserialize(ReadBufferContext context)
        {
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++)
            {
                var record = new ClientQueryResult();
                record.Deserialize(context);
                Clients.Add(record);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteInteger(Clients.Count);
            foreach(var client in Clients)
                client.Serialize(context);
        }
    }
}
