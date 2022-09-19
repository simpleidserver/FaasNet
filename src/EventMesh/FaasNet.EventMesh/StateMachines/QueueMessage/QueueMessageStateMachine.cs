using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client.StateMachines.QueueMessage;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.StateMachines;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.QueueMessage
{
    public class QueueMessageStateMachine : IStateMachine
    {
        private readonly IQueueMessageStateMachineStore _store;

        public QueueMessageStateMachine(IQueueMessageStateMachineStore store)
        {
            _store = store;
        }

        public Task Apply(ICommand cmd, CancellationToken cancellationToken)
        {
            switch(cmd)
            {
                case AddQueueMessageCommand addQueueMessage:
                    _store.Add(new QueueMessageRecord { Data = addQueueMessage.Data, Id = addQueueMessage.Id, Topic = addQueueMessage.Topic });
                    break;
            }

            return Task.CompletedTask;
        }

        public Task BulkUpload(IEnumerable<IRecord> records, CancellationToken cancellationToken)
        {
            return _store.BulkUpload(records.Cast<QueueMessageRecord>(), cancellationToken);
        }

        public async Task Commit(CancellationToken cancellationToken)
        {
            await _store.SaveChanges(cancellationToken);
        }

        public async Task<IQueryResult> Query(IQuery query, CancellationToken cancellationToken)
        {
            switch(query)
            {
                case GetQueueMessageQuery getQueueMessage:
                    var result = await _store.Get(getQueueMessage.Offset, cancellationToken);
                    if (result == null) return new GetQueueMessageQueryResult();
                    return new GetQueueMessageQueryResult(new QueueMessageQueryResult
                    {
                        Id = result.Id,
                        Data = result.Data,
                        Topic = result.Topic
                    });
            }

            return null;
        }

        public IEnumerable<(IEnumerable<IEnumerable<byte>>, int)> Snapshot(int nbRecords)
        {
            var name = typeof(QueueMessageRecord).AssemblyQualifiedName;
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

    public class QueueMessageRecord : IRecord
    {
        public string Id { get; set; }
        public string Topic { get; set; }
        public CloudEvent Data { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Topic = context.NextString();
            Data = context.NextCloudEvent();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Topic);
            Data.Serialize(context);
        }
    }
}
