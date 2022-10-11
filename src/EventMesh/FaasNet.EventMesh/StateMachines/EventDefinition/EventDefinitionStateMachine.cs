using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.StateMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.EventDefinition
{
    public class EventDefinitionStateMachine : IStateMachine
    {
        private readonly IEventDefinitionStateMachineStore _store;

        public EventDefinitionStateMachine(IEventDefinitionStateMachineStore store)
        {
            _store = store;
        }

        public async Task Apply(ICommand cmd, CancellationToken cancellationToken)
        {
            switch(cmd)
            {
                case AddEventDefinitionCommand addEventDefinition:
                    var existingVpnDef = await _store.Get(addEventDefinition.Id, addEventDefinition.Vpn, cancellationToken);
                    if (existingVpnDef != null) return;
                    var record = new EventDefinitionRecord
                    {
                        Id = addEventDefinition.Id,
                        CreateDateTime = DateTime.UtcNow,
                        JsonSchema = addEventDefinition.JsonSchema,
                        Vpn = addEventDefinition.Vpn
                    };
                    record.Sources.Add(addEventDefinition.Source);
                    record.Targets.Add(addEventDefinition.Target);
                    _store.Add(record);
                    break;
            }
        }

        public Task BulkUpload(IEnumerable<IRecord> records, CancellationToken cancellationToken)
        {
            return _store.BulkUpload(records.Cast<EventDefinitionRecord>(), cancellationToken);
        }

        public Task Commit(CancellationToken cancellationToken)
        {
            return _store.SaveChanges(cancellationToken);
        }

        public async Task<IQueryResult> Query(IQuery query, CancellationToken cancellationToken)
        {
            switch(query)
            {
                case GetEventDefinitionQuery getEventDef:
                    var result = await _store.Get(getEventDef.Id, getEventDef.Vpn, cancellationToken);
                    if (result == null) return new GetEventDefinitionQueryResult();
                    return new GetEventDefinitionQueryResult(Transform(result));
            }

            return null;
        }

        public IEnumerable<(IEnumerable<IEnumerable<byte>>, int)> Snapshot(int nbRecords)
        {
            var name = typeof(EventDefinitionRecord).AssemblyQualifiedName;
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

        private static EventDefinitionQueryResult Transform(EventDefinitionRecord client)
        {
            return new EventDefinitionQueryResult
            {

            };
        }
    }

    public class EventDefinitionRecord : IRecord
    {
        public string Id { get; set; }
        public string Vpn { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string JsonSchema { get; set; }
        public ICollection<string> Sources { get; set; } = new List<string>();
        public ICollection<string> Targets { get; set; } = new List<string>();

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Vpn = context.NextString();
            CreateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks);
            JsonSchema = context.NextString();
            var nbSources = context.NextInt();
            for (var i = 0; i < nbSources; i++) Sources.Add(context.NextString());
            var nbTargets = context.NextInt();
            for (var i = 0; i < nbTargets; i++) Targets.Add(context.NextString());
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Vpn);
            context.WriteTimeSpan(TimeSpan.FromTicks(CreateDateTime.Ticks));
            context.WriteString(JsonSchema);
            context.WriteInteger(Sources.Count);
            foreach (var source in Sources) context.WriteString(source);
            context.WriteInteger(Targets.Count);
            foreach (var target in Targets) context.WriteString(target);
        }
    }
}
