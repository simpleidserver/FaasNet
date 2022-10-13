using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.StateMachines;
using System;
using System.Collections.Generic;
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
                    var existingClient = await _store.Get(addClient.Id, addClient.Vpn, cancellationToken);
                    if (existingClient != null) return;
                    _store.Add(new ClientRecord 
                    { 
                        ClientSecret = addClient.ClientSecret, 
                        Id = addClient.Id, 
                        Purposes = addClient.Purposes, 
                        SessionExpirationTimeMS = addClient.SessionExpirationTimeMS, 
                        Vpn = addClient.Vpn,
                        CreateDateTime = DateTime.UtcNow,
                        CoordinateX = addClient.CoordinateX,
                        CoordinateY = addClient.CoordinateY
                    });
                    await _store.SaveChanges(cancellationToken);
                    break;
                case BulkUpdateClientCommand updateClient:
                    foreach(var client in updateClient.Clients)
                    {
                        var clientToUpdate = await _store.Get(client.Id, updateClient.Vpn, cancellationToken);
                        if (clientToUpdate == null) continue;;
                        clientToUpdate.Update(client);
                        _store.Update(clientToUpdate);
                    }

                    await _store.SaveChanges(cancellationToken);
                    break;
                case RemoveClientCommand removeClient:
                    var ec = await _store.Get(removeClient.ClientId, removeClient.Vpn, cancellationToken);
                    if (ec == null) return;
                    _store.Remove(ec);
                    await _store.SaveChanges(cancellationToken);
                    // Use masstransit to send the integration event "ClientRemovedEvent"
                    // In the handler : remove all the links. If there is an exception then the message can be enqueued later on.
                    break;
                case AddTargetCommand addTarget:
                    var cl = await _store.Get(addTarget.ClientId, addTarget.Vpn, cancellationToken);
                    if (cl == null) return;
                    if (!cl.Targets.Any(t => t.Target == addTarget.Target))
                    {
                        cl.Targets.Add(new ClientTarget
                        {
                            Target = addTarget.Target,
                            EventId = addTarget.EventDefId
                        });
                        _store.Update(cl);
                        await _store.SaveChanges(cancellationToken);
                    }
                    break;
                case RemoveTargetCommand removeTarget:
                    var c = await _store.Get(removeTarget.ClientId, removeTarget.Vpn, cancellationToken);
                    if (c == null) return;
                    var target = c.Targets.SingleOrDefault(t => t.Target == removeTarget.Target && t.EventId == removeTarget.EventId);
                    if (target != null)
                    {
                        c.Targets.Remove(target);
                        _store.Update(c);
                        await _store.SaveChanges(cancellationToken);
                    }
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
                    var result = await _store.Get(getClient.Id, getClient.Vpn, cancellationToken);
                    if (result == null) return new GetClientQueryResult();
                    return new GetClientQueryResult(Transform(result));
                case GetAllClientsQuery getClients:
                    var res = await _store.Find(getClients.Filter, cancellationToken);
                    return new GenericSearchQueryResult<ClientQueryResult>
                    {
                        TotalPages = res.TotalPages,
                        TotalRecords = res.TotalRecords,
                        Records = res.Records.Select(r => Transform(r))
                    };
                case FindClientsByNameQuery findClientsByName:
                    var findResult = await _store.Find(findClientsByName.Name, cancellationToken);
                    return new FindClientsByNameQueryResult
                    {
                        Content = findResult
                    };
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

        private static ClientQueryResult Transform(ClientRecord client)
        {
            return new ClientQueryResult
            {
                Id = client.Id,
                ClientSecret = client.ClientSecret,
                Purposes = client.Purposes,
                SessionExpirationTimeMS = client.SessionExpirationTimeMS,
                Vpn = client.Vpn,
                CreateDateTime = client.CreateDateTime,
                CoordinateX = client.CoordinateX,
                CoordinateY = client.CoordinateY,
                Targets = client.Targets.Select(t => new ClientTargetResult
                {
                    EventId = t.EventId,
                    Target = t.Target
                }).ToList()
            };
        }
    }

    public class ClientRecord : IRecord
    {
        public string Id { get; set; }
        public string ClientSecret { get; set; }
        public string Vpn { get; set; }
        public int SessionExpirationTimeMS { get; set; }
        public ICollection<ClientPurposeTypes> Purposes { get; set; } = new List<ClientPurposeTypes>();
        public DateTime CreateDateTime { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public ICollection<ClientTarget> Targets { get; set; } = new List<ClientTarget>();

        public void Update(UpdateClient cmd)
        {
            CoordinateX = cmd.CoordinateX;
            CoordinateY = cmd.CoordinateY;
            Targets = cmd.Targets.Select(t => new ClientTarget
            {
                Target = t.Target,
                EventId = t.EventId
            }).ToList();
        }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            ClientSecret = context.NextString();
            Vpn = context.NextString();
            SessionExpirationTimeMS = context.NextInt();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++) Purposes.Add((ClientPurposeTypes)context.NextInt());
            CreateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks);
            CoordinateX = context.NextDouble();
            CoordinateY = context.NextDouble();
            nb = context.NextInt();
            for (var i = 0; i < nb; i++)
            {
                var target = new ClientTarget();
                target.Deserialize(context);
                Targets.Add(target);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(ClientSecret);
            context.WriteString(Vpn);
            context.WriteInteger(SessionExpirationTimeMS);
            context.WriteInteger(Purposes.Count);
            foreach (var purpose in Purposes) context.WriteInteger((int)purpose);
            context.WriteTimeSpan(TimeSpan.FromTicks(CreateDateTime.Ticks));
            context.WriteDouble(CoordinateX);
            context.WriteDouble(CoordinateY);
            context.WriteInteger(Targets.Count());
            foreach (var target in Targets) target.Serialize(context);
        }
    }

    public class ClientTarget : ISerializable
    {
        public string Target { get; set; }
        public string EventId { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Target = context.NextString();
            EventId = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Target);
            context.WriteString(EventId);
        }
    }
}
