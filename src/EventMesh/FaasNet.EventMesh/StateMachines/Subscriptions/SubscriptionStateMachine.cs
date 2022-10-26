using FaasNet.EventMesh.Client.StateMachines.Subscription;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.StateMachines;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Subscriptions
{
    public class SubscriptionStateMachine : IStateMachine
    {
        private readonly ISubscriptionStateMachineStore _store;

        public SubscriptionStateMachine(ISubscriptionStateMachineStore store)
        {
            _store = store;
        }

        public async Task Apply(ICommand cmd, CancellationToken cancellationToken)
        {
            switch(cmd)
            {
                case AddSubscriptionCommand addSubscription:
                    _store.Add(SubscriptionRecord.Create(addSubscription));
                    await _store.SaveChanges(cancellationToken);
                    break;
                case RemoveSubscriptionCommand removeSubscription:
                    var record = await _store.Get(removeSubscription.ClientId, removeSubscription.EventId, removeSubscription.Vpn, cancellationToken);
                    if (record == null) return;
                    _store.Delete(record);
                    await _store.SaveChanges(cancellationToken);
                    break;
            }
        }

        public Task BulkUpload(IEnumerable<IRecord> records, CancellationToken cancellationToken)
        {
            return _store.BulkUpload(records.Cast<SubscriptionRecord>(), cancellationToken);
        }

        public Task Commit(CancellationToken cancellationToken)
        {
            return _store.SaveChanges(cancellationToken);
        }

        public async Task<IQueryResult> Query(IQuery query, CancellationToken cancellationToken)
        {
            switch(query)
            {
                case GetAllSubscriptionsQuery getAllSubscriptions:
                    var result = await _store.GetAll(getAllSubscriptions.TopicFilter, getAllSubscriptions.Vpn, cancellationToken);
                    return new GetAllSubscriptionsQueryResult
                    {
                        Subscriptions = result.Select(r => Transform(r)).ToList()
                    };
            }

            return null;
        }

        public IEnumerable<(IEnumerable<IEnumerable<byte>>, int)> Snapshot(int nbRecords)
        {
            var name = typeof(SubscriptionRecord).AssemblyQualifiedName;
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

        private static SubscriptionResult Transform(SubscriptionRecord record)
        {
            return new SubscriptionResult
            {
                ClientId = record.ClientId,
                EventId = record.EventId,
                Id = record.Id,
                Topic = record.Topic,
                Vpn = record.Vpn
            };
        }
    }

    public class SubscriptionRecord : IRecord
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string EventId { get; set; }
        public string Vpn { get; set; }
        public string Topic { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            ClientId = context.NextString();
            EventId = context.NextString();
            Vpn = context.NextString();
            Topic = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(ClientId);
            context.WriteString(EventId);
            context.WriteString(Vpn);
            context.WriteString(Topic);
        }

        public static SubscriptionRecord Create(AddSubscriptionCommand cmd)
        {
            return new SubscriptionRecord
            {
                Id = cmd.Id,
                ClientId = cmd.ClientId,
                EventId = cmd.EventId,
                Topic = cmd.Topic,
                Vpn = cmd.Vpn
            };
        }
    }
}
