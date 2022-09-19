using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.StateMachines;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Client
{
    public class ClientStateMachine : IStateMachine
    {
        private readonly IClientStateMachineStore _store;
        
        public ClientStateMachine(IClientStateMachineStore store)
        {
            _store = store;
        }

        public async Task Apply(ICommand cmd, CancellationToken cancellationToken)
        {
            switch(cmd)
            {
                case AddClientCommand addClient:
                    var client = await _store.Get(addClient.Id, cancellationToken);
                    if (client != null) return;
                    _store.Add(new ClientRecord { ClientSecret = addClient.ClientSecret, Id = addClient.Id, Purposes = addClient.Purposes, SessionExpirationTimeMS = addClient.SessionExpirationTimeMS, Vpn = addClient.Vpn });
                    break;
            }
        }

        public Task BulkUpload(IEnumerable<IRecord> records, CancellationToken cancellationToken)
        {
            return _store.BulkUpload(records.Cast<ClientRecord>(), cancellationToken);
        }

        public async Task Commit(CancellationToken cancellationToken)
        {
            await _store.SaveChanges(cancellationToken);
        }

        public async Task<IQueryResult> Query(IQuery query, CancellationToken cancellationToken)
        {
            switch(query)
            {
                case GetClientQuery getClient:
                    var result = await _store.Get(getClient.Id, cancellationToken);
                    if (result == null) return new GetClientQueryResult();
                    return new GetClientQueryResult(new ClientQueryResult { Id = result.Id, ClientSecret = result.ClientSecret, Purposes = result.Purposes, SessionExpirationTimeMS = result.SessionExpirationTimeMS, Vpn = result.Vpn });
                case GetAllClientsQuery getClients:
                    var clients = await _store.GetAll(cancellationToken);
                    return new GetAllClientsQueryResult { Clients = clients.Select(c => new ClientQueryResult
                    {
                        Id = c.Id,
                        ClientSecret = c.ClientSecret,
                        Purposes = c.Purposes,
                        SessionExpirationTimeMS = c.SessionExpirationTimeMS,
                        Vpn = c.Vpn
                    }).ToList() };
            }

            return null;
        }

        public IEnumerable<(IEnumerable<IEnumerable<byte>>, int)> Snapshot(int nbRecords)
        {
            var name = typeof(ClientRecord).AssemblyQualifiedName;
            foreach (var records in _store.GetAll(nbRecords))
            {
                yield return (records.Item1.Select(r =>
                {
                    var ctx = new WriteBufferContext();
                    ctx.WriteString(name);
                    r.Serialize(ctx);
                    return (IEnumerable<byte>)ctx.Buffer;
                }), records.Item2);
            }
        }

        public Task Truncate(CancellationToken cancellationToken)
        {
            return _store.Truncate(cancellationToken);
        }
    }

    public class ClientRecord : IRecord
    {
        public string Id { get; set; }
        public string ClientSecret { get; set; }
        public string Vpn { get; set; }
        public int SessionExpirationTimeMS { get; set; }
        public ICollection<ClientPurposeTypes> Purposes { get; set; } = new List<ClientPurposeTypes>();


        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            ClientSecret = context.NextString();
            Vpn = context.NextString();
            SessionExpirationTimeMS = context.NextInt();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++) Purposes.Add((ClientPurposeTypes)context.NextInt());
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(ClientSecret);
            context.WriteString(Vpn);
            context.WriteInteger(SessionExpirationTimeMS);
            context.WriteInteger(Purposes.Count);
            foreach (var purpose in Purposes) context.WriteInteger((int)purpose);
        }
    }

}
