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
                    record.Links.Add(new EventDefinitionLink
                    {
                        Source = addEventDefinition.Source,
                        Target = addEventDefinition.Target
                    });
                    _store.Add(record);
                    await _store.SaveChanges(cancellationToken);
                    break;
                case UpdateEventDefinitionCommand updateEventDefinition:
                    var evtDef = await _store.Get(updateEventDefinition.Id, updateEventDefinition.Vpn, cancellationToken);
                    if (evtDef == null) return;
                    evtDef.JsonSchema = updateEventDefinition.JsonSchema;
                    _store.Update(evtDef);
                    await _store.SaveChanges(cancellationToken);
                    break;
                case RemoveLinkEventDefinitionCommand removeEventDefinitionCommand:
                    var ed = await _store.Get(removeEventDefinitionCommand.Id, removeEventDefinitionCommand.Vpn, cancellationToken);
                    if (ed == null) return;
                    var link = ed.Links.SingleOrDefault(l => l.Source == removeEventDefinitionCommand.Source && l.Target == removeEventDefinitionCommand.Target);
                    if (link != null)
                    {
                        ed.Links.Remove(link);
                        _store.Update(ed);
                        await _store.SaveChanges(cancellationToken);
                    }

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

        private static EventDefinitionQueryResult Transform(EventDefinitionRecord evtDef)
        {
            return new EventDefinitionQueryResult
            {
                Id = evtDef.Id,
                JsonSchema = evtDef.JsonSchema,
                Vpn = evtDef.Vpn
            };
        }
    }

    public class EventDefinitionRecord : IRecord
    {
        public string Id { get; set; }
        public string Vpn { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string JsonSchema { get; set; }
        public ICollection<EventDefinitionLink> Links { get; set; } = new List<EventDefinitionLink>();

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Vpn = context.NextString();
            CreateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks);
            JsonSchema = context.NextString();
            var nbLinks = context.NextInt();
            for (var i = 0; i < nbLinks; i++)
            {
                var link = new EventDefinitionLink();
                link.Deserialize(context);
                Links.Add(link);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Vpn);
            context.WriteTimeSpan(TimeSpan.FromTicks(CreateDateTime.Ticks));
            context.WriteString(JsonSchema);
            context.WriteInteger(Links.Count);
            foreach (var link in Links) link.Serialize(context);
        }
    }

    public class EventDefinitionLink : ISerializable
    {
        public string Source { get; set; }
        public string Target { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Source = context.NextString();
            Target = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Source);
            context.WriteString(Target);
        }
    }
}
