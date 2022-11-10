using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.Session;
using FaasNet.EventMesh.StateMachines.Queue;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.StateMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Session
{
    public class SessionStateMachine : IStateMachine
    {
        private readonly ISessionStateMachineStore _store;

        public SessionStateMachine(ISessionStateMachineStore store)
        {
            _store = store;
        }

        public string Name => nameof(SessionStateMachine);

        public async Task Apply(ICommand cmd, CancellationToken cancellationToken)
        {
            switch (cmd)
            {
                case AddSessionCommand addSession:
                    var session = await _store.Get(addSession.Id, cancellationToken);
                    if (session != null) return;
                    _store.Add(new SessionRecord 
                    { 
                        ClientId = addSession.ClientId, 
                        Vpn = addSession.Vpn,
                        ClientPurpose = addSession.ClientPurpose, 
                        ExpirationTime = addSession.ExpirationTime, 
                        Id = addSession.Id, 
                        QueueName = addSession.QueueName
                    });

                    break;
            }
        }

        public Task BulkUpload(IEnumerable<IRecord> records, CancellationToken cancellationToken)
        {
            return _store.BulkUpload(records.Cast<SessionRecord>(), cancellationToken);
        }

        public async Task Commit(CancellationToken cancellationToken)
        {
            await _store.SaveChanges(cancellationToken);
        }

        public async Task<IQueryResult> Query(IQuery query, CancellationToken cancellationToken)
        {
            switch (query)
            {
                case GetSessionQuery getSession:
                    var result = await _store.Get(getSession.Id, cancellationToken);
                    if (result == null) return new GetSessionQueryResult();
                    return new GetSessionQueryResult(Transform(result));
                case SearchSessionsQuery searchSessions:
                    var res = await _store.Find(searchSessions.ClientId, searchSessions.Vpn, searchSessions.Filter, cancellationToken);
                    return new GenericSearchQueryResult<SessionQueryResult>
                    {
                        TotalPages = res.TotalPages,
                        TotalRecords = res.TotalRecords,
                        Records = res.Records.Select(res => Transform(res))
                    };
            }

            return null;
        }

        public IEnumerable<(IEnumerable<IEnumerable<byte>>, int)> Snapshot(int nbRecords)
        {
            var name = typeof(SessionRecord).AssemblyQualifiedName;
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

        private static SessionQueryResult Transform(SessionRecord session)
        {
            return new SessionQueryResult 
            { 
                ClientId = session.ClientId, 
                ClientPurpose = session.ClientPurpose, 
                ExpirationTime = session.ExpirationTime, 
                Id = session.Id, 
                QueueName = session.QueueName,
                Vpn = session.Vpn
            };
        }
    }

    public class SessionRecord : IRecord
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string Vpn { get; set; }
        public ClientPurposeTypes ClientPurpose { get; set; }
        public TimeSpan ExpirationTime { get; set; }
        public string QueueName { get; set; }
        public bool IsValid => DateTime.UtcNow.Ticks < ExpirationTime.Ticks;

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            ClientId = context.NextString();
            Vpn = context.NextString();
            ClientPurpose = (ClientPurposeTypes)context.NextInt();
            ExpirationTime = context.NextTimeSpan().Value;
            QueueName = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(ClientId);
            context.WriteString(Vpn);
            context.WriteInteger((int)ClientPurpose);
            context.WriteTimeSpan(ExpirationTime);
            context.WriteString(QueueName);
        }
    }
}
