using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Queue;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.StateMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Queue
{
    public class QueueStateMachine : IStateMachine
    {
        private readonly IQueueStateMachineStore _store;

        public QueueStateMachine(IQueueStateMachineStore store)
        {
            _store = store;
        }

        public async Task Apply(ICommand cmd, CancellationToken cancellationToken)
        {
            switch (cmd)
            {
                case AddQueueCommand addQueue:
                    var client = await _store.Get(addQueue.QueueName, addQueue.Vpn, cancellationToken);
                    if (client != null) return;
                    _store.Add(new QueueRecord { QueueName = addQueue.QueueName, Vpn = addQueue.Vpn, TopicFilter = addQueue.TopicFilter, CreateDateTime = DateTime.UtcNow });
                    break;
            }
        }

        public Task BulkUpload(IEnumerable<IRecord> records, CancellationToken cancellationToken)
        {
            return _store.BulkUpload(records.Cast<QueueRecord>(), cancellationToken);
        }

        public async Task Commit(CancellationToken cancellationToken)
        {
            await _store.SaveChanges(cancellationToken);
        }

        public async Task<IQueryResult> Query(IQuery query, CancellationToken cancellationToken)
        {
            switch (query)
            {
                case GetQueueQuery getQueue:
                    var result = await _store.Get(getQueue.QueueName, getQueue.Vpn, cancellationToken);
                    if (result == null) return new GetQueueQueryResult();
                    return new GetQueueQueryResult(Transform(result));
                case SearchQueuesQuery searchQueuesQuery:
                    var queues = await _store.Search(searchQueuesQuery.Vpn, searchQueuesQuery.TopicMessage, cancellationToken);
                    return new SearchQueuesQueryResult { Queues = queues.Select(q => Transform(q)).ToList()};
                case FindQueuesQuery findQueuesQuery:
                    var findResult = await _store.Find(findQueuesQuery.Filter, cancellationToken);
                    return new GenericSearchQueryResult<QueueQueryResult>
                    {
                        TotalPages = findResult.TotalPages,
                        TotalRecords = findResult.TotalRecords,
                        Records = findResult.Records.Select(r => Transform(r))
                    };
            }

            return null;
        }

        public IEnumerable<(IEnumerable<IEnumerable<byte>>, int)> Snapshot(int nbRecords)
        {
            var name = typeof(QueueRecord).AssemblyQualifiedName;
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

        private static QueueQueryResult Transform(QueueRecord record)
        {
            return new QueueQueryResult
            {
                Vpn = record.Vpn,
                QueueName = record.QueueName,
                TopicFilter = record.TopicFilter,
                CreateDateTime = record.CreateDateTime
            };
        }
    }

    public class QueueRecord : IRecord
    {
        public string Vpn { get; set; }
        public string QueueName { get; set; }
        public string TopicFilter { get; set; }
        public DateTime? CreateDateTime { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Vpn = context.NextString();
            QueueName = context.NextString();
            TopicFilter = context.NextString();
            CreateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks);
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteString(QueueName);
            context.WriteString(TopicFilter);
            context.WriteTimeSpan(TimeSpan.FromTicks(CreateDateTime.GetValueOrDefault().Ticks));
        }
    }
}
