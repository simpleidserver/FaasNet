using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.Partition;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.Infos;
using FaasNet.RaftConsensus.Core.StateMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Client
{
    public class ClientStateMachine : BaseStateMachine
    {
        private readonly IClientStateMachineStore _store;
        
        public ClientStateMachine(IClientStateMachineStore store, IPeerInfoStore peerInfoStore, IMediator mediator) : base(peerInfoStore, mediator)
        {
            _store = store;
        }

        public override async Task Apply(ICommand cmd, CancellationToken cancellationToken)
        {
            switch (cmd)
            {
                case AddClientCommand addClient:
                    {
                        var client = await _store.Get(addClient.Id, addClient.Vpn, cancellationToken);
                        if (client != null) return;
                        _store.Add(new ClientRecord
                        {
                            ClientSecret = addClient.ClientSecret,
                            Id = addClient.Id,
                            Purposes = addClient.Purposes,
                            SessionExpirationTimeMS = addClient.SessionExpirationTimeMS,
                            Vpn = addClient.Vpn,
                            CreateDateTime = DateTime.UtcNow
                        });
                        await _store.SaveChanges(cancellationToken);
                        await PropagateIntegrationEvent(new ClientAdded
                        {
                            ClientId = addClient.Id,
                            Vpn = addClient.Vpn
                        }, cancellationToken);
                    }
                    break;
                case AddSourceCommand addSource:
                    {
                        var client = await _store.Get(addSource.ClientId, addSource.Vpn, cancellationToken);
                        if (client == null) return;
                        if(client.Handle(addSource))
                        {
                            _store.Update(client);
                            await _store.SaveChanges(cancellationToken);
                        }

                    }
                    break;
                case RemoveSourceCommand removeSource:
                    {
                        var client = await _store.Get(removeSource.ClientId, removeSource.Vpn, cancellationToken);
                        if (client == null) return;
                        if (client.Handle(removeSource))
                        {
                            _store.Update(client);
                            await _store.SaveChanges(cancellationToken);
                        }

                    }
                    break;
                case AddTargetCommand addTarget:
                    {
                        var client = await _store.Get(addTarget.ClientId, addTarget.Vpn, cancellationToken);
                        if (client == null) return;
                        if (client.Handle(addTarget))
                        {
                            _store.Update(client);
                            await _store.SaveChanges(cancellationToken);
                        }

                    }
                    break;
                case RemoveTargetCommand removeTarget:
                    {
                        var client = await _store.Get(removeTarget.ClientId, removeTarget.Vpn, cancellationToken);
                        if (client == null) return;
                        if (client.Handle(removeTarget))
                        {
                            _store.Update(client);
                            await _store.SaveChanges(cancellationToken);
                        }

                    }
                    break;
            }
        }

        public override Task BulkUpload(IEnumerable<IRecord> records, CancellationToken cancellationToken)
        {
            return _store.BulkUpload(records.Cast<ClientRecord>(), cancellationToken);
        }

        public override async Task Commit(CancellationToken cancellationToken)
        {
            await _store.SaveChanges(cancellationToken);
        }

        public override async Task<IQueryResult> Query(IQuery query, CancellationToken cancellationToken)
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

        public override IEnumerable<(IEnumerable<IEnumerable<byte>>, int)> Snapshot(int nbRecords)
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

        public override Task Truncate(CancellationToken cancellationToken)
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
                Sources = client.Sources.Select(t => new ClientLinkResult
                {
                    EventId = t.EventId,
                    ClientId = t.ClientId
                }).ToList(),
                Targets = client.Targets.Select(t => new ClientLinkResult
                {
                    EventId = t.EventId,
                    ClientId = t.ClientId
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
        public ICollection<ClientLink> Sources { get; set; } = new List<ClientLink>();
        public ICollection<ClientLink> Targets { get; set; } = new List<ClientLink>();

        public bool Handle(AddSourceCommand cmd)
        {
            var source = Sources.SingleOrDefault(s => s.ClientId == cmd.Source);
            if (source != null) return false;
            Sources.Add(new ClientLink
            {
                ClientId = cmd.Source,
                EventId = cmd.EventDefId
            });
            return true;
        }

        public bool Handle(RemoveSourceCommand cmd)
        {
            var source = Sources.SingleOrDefault(s => s.ClientId == cmd.Source);
            if (source == null) return false;
            Sources.Remove(source);
            return true;
        }

        public bool Handle(AddTargetCommand cmd)
        {
            var source = Targets.SingleOrDefault(s => s.ClientId == cmd.Target);
            if (source != null) return false;
            Targets.Add(new ClientLink
            {
                ClientId = cmd.Target,
                EventId = cmd.EventDefId
            });
            return true;
        }

        public bool Handle(RemoveTargetCommand cmd)
        {
            var target = Targets.SingleOrDefault(s => s.ClientId == cmd.Target);
            if (target == null) return false;
            Targets.Remove(target);
            return true;
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
            nb = context.NextInt();
            for (var i = 0; i < nb; i++)
            {
                var source = new ClientLink();
                source.Deserialize(context);
                Sources.Add(source);
            }

            nb = context.NextInt();
            for(var i = 0; i < nb; i++)
            {
                var target = new ClientLink();
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
            context.WriteInteger(Sources.Count());
            foreach (var source in Sources) source.Serialize(context);
            context.WriteInteger(Targets.Count());
            foreach (var target in Targets) target.Serialize(context);
        }
    }

    public class ClientLink : ISerializable
    {
        public string ClientId { get; set; }
        public string EventId { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            ClientId = context.NextString();
            EventId = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(ClientId);
            context.WriteString(EventId);
        }
    }
}
