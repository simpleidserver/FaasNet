using FaasNet.EventMesh.Client.StateMachines.Queue;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.StateMachines;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Queue
{
    public class QueueStateMachine : IStateMachine
    {
        private readonly IStateMachineRecordStore<QueueRecord> _store;

        public QueueStateMachine(IStateMachineRecordStore<QueueRecord> store)
        {
            _store = store;
        }

        public async Task Apply(ICommand cmd, CancellationToken cancellationToken)
        {
            switch (cmd)
            {
                case AddQueueCommand addQueue:
                    var client = await _store.Get(addQueue.QueueName, cancellationToken);
                    if (client != null) return;
                    _store.Add(new QueueRecord { QueueName = addQueue.QueueName, TopicFilter = addQueue.TopicFilter });
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
                    var result = await _store.Get(getQueue.QueueName, cancellationToken);
                    if (result == null) return null;
                    return new GetQueueQueryResult { QueueName = result.QueueName, TopicFilter = result.TopicFilter };
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
    }

    public class QueueRecord : IRecord
    {
        public string QueueName { get; set; }
        public string TopicFilter { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            QueueName = context.NextString();
            TopicFilter = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(QueueName);
            context.WriteString(TopicFilter);
        }
    }
}
