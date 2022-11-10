using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.StateMachines.Counter;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.StateMachines.Counter
{
    public class CounterStateMachine : IStateMachine
    {
        private readonly IStateMachineRecordStore<CounterRecord> _store;

        public string Name => nameof(CounterStateMachine);

        public CounterStateMachine(IStateMachineRecordStore<CounterRecord> store)
        {
            _store = store;
        }

        public async Task Apply(ICommand cmd, CancellationToken cancellationToken)
        {
            switch (cmd)
            {
                case IncrementCounter increment:
                    var counter = await _store.Get(increment.Id, cancellationToken);
                    if (counter == null) _store.Add(new CounterRecord { Id = increment.Id, Value = increment.Value });
                    else
                    {
                        counter.Value += increment.Value;
                        _store.Update(counter);
                    }
                    break;
            }
        }

        public async Task<IQueryResult> Query(IQuery query, CancellationToken cancellationToken)
        {
            switch (query)
            {
                case GetCounterQuery getQuery:
                    var record = await _store.Get(getQuery.Id, cancellationToken);
                    if (record == null) return GetCounterQueryResult.NotFound();
                    return GetCounterQueryResult.OK(record.Id, record.Value);
            }

            return null;
        }

        public IEnumerable<(IEnumerable<IEnumerable<byte>>, int)> Snapshot(int nbRecords)
        {
            var name = typeof(CounterRecord).AssemblyQualifiedName;
            foreach(var records in _store.GetAll(nbRecords))
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

        public Task Commit(CancellationToken cancellationToken)
        {
            return _store.SaveChanges(cancellationToken);
        }

        public Task Truncate(CancellationToken cancellationToken)
        {
            return _store.Truncate(cancellationToken);
        }

        public Task BulkUpload(IEnumerable<IRecord> records, CancellationToken cancellationToken)
        {
            return _store.BulkUpload(records.Cast<CounterRecord>(), cancellationToken);
        }
    }

    public class CounterRecord : IRecord
    {
        public string Id { get; set; }
        public long Value { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteLong(Value);
        }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Value = context.NextLong();
        }
    }
}
