using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
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

namespace FaasNet.EventMesh.StateMachines.EventDefinition
{
    public class EventDefinitionStateMachine : BaseStateMachine
    {
        private readonly IEventDefinitionStateMachineStore _store;

        public EventDefinitionStateMachine(IEventDefinitionStateMachineStore store, IPeerInfoStore peerInfoStore, IMediator mediator) : base(peerInfoStore, mediator)
        {
            _store = store;
        }

        public async override Task Apply(ICommand cmd, CancellationToken cancellationToken)
        {
            switch(cmd)
            {
                case AddEventDefinitionCommand addEventDefinition:
                    {
                        var evtDef = await _store.Get(addEventDefinition.Id, addEventDefinition.Vpn, cancellationToken);
                        if (evtDef != null) return;
                        var record = new EventDefinitionRecord
                        {
                            Id = addEventDefinition.Id,
                            CreateDateTime = DateTime.UtcNow,
                            UpdateDateTime = DateTime.UtcNow,
                            Topic = addEventDefinition.Topic,
                            JsonSchema = addEventDefinition.JsonSchema,
                            Description = addEventDefinition.Description,
                            Vpn = addEventDefinition.Vpn
                        };
                        _store.Add(record);
                        await _store.SaveChanges(cancellationToken);
                    }
                    break;
                case UpdateEventDefinitionCommand updateEventDefinition:
                    {
                        var evtDef = await _store.Get(updateEventDefinition.Id, updateEventDefinition.Vpn, cancellationToken);
                        if (evtDef == null) return;
                        evtDef.JsonSchema = updateEventDefinition.JsonSchema;
                        _store.Update(evtDef);
                        await _store.SaveChanges(cancellationToken);
                    }
                    break;
                case RemoveLinkEventDefinitionCommand removeEventDefinitionCommand:
                    {
                        var evtDef = await _store.Get(removeEventDefinitionCommand.Id, removeEventDefinitionCommand.Vpn, cancellationToken);
                        if (evtDef == null) return;
                        if (evtDef.Handle(removeEventDefinitionCommand))
                        {
                            _store.Update(evtDef);
                            await _store.SaveChanges(cancellationToken);
                        }

                    }
                    break;
                case AddLinkEventDefinitionCommand addLinkEventDefinitionCommand:
                    {
                        var evtDef = await _store.Get(addLinkEventDefinitionCommand.Id, addLinkEventDefinitionCommand.Vpn, cancellationToken);
                        if (evtDef == null) return;
                        if(evtDef.Handle(addLinkEventDefinitionCommand))
                        {
                            _store.Update(evtDef);
                            await _store.SaveChanges(cancellationToken);
                        }
                    }
                    break;
            }
        }

        public override Task BulkUpload(IEnumerable<IRecord> records, CancellationToken cancellationToken)
        {
            return _store.BulkUpload(records.Cast<EventDefinitionRecord>(), cancellationToken);
        }

        public override Task Commit(CancellationToken cancellationToken)
        {
            return _store.SaveChanges(cancellationToken);
        }

        public override async Task<IQueryResult> Query(IQuery query, CancellationToken cancellationToken)
        {
            switch(query)
            {
                case GetEventDefinitionQuery getEventDef:
                    {
                        var result = await _store.Get(getEventDef.Id, getEventDef.Vpn, cancellationToken);
                        if (result == null) return new GetEventDefinitionQueryResult();
                        return new GetEventDefinitionQueryResult(Transform(result));
                    }
                case GetEventDefsQuery getEventDefsQuery:
                    {
                        var result = await _store.Find(getEventDefsQuery.Filter, cancellationToken);
                        return new GenericSearchQueryResult<EventDefinitionQueryResult>
                        {
                            Records = result.Records.Select(r => Transform(r)),
                            TotalPages = result.TotalPages,
                            TotalRecords = result.TotalRecords
                        };
                    }
            }

            return null;
        }

        public override IEnumerable<(IEnumerable<IEnumerable<byte>>, int)> Snapshot(int nbRecords)
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

        public override Task Truncate(CancellationToken cancellationToken)
        {
            return _store.Truncate(cancellationToken);
        }

        private static EventDefinitionQueryResult Transform(EventDefinitionRecord evtDef)
        {
            return new EventDefinitionQueryResult
            {
                Id = evtDef.Id,
                JsonSchema = evtDef.JsonSchema,
                Description = evtDef.Description,
                Vpn = evtDef.Vpn,
                Topic = evtDef.Topic,
                CreateDateTime = evtDef.CreateDateTime,
                UpdateDateTime = evtDef.UpdateDateTime,
                Links = evtDef.Links.Select(l => new EventDefinitionLinkResult
                {
                    Source = l.Source,
                    Target = l.Target
                }).ToList()
            };
        }
    }

    public class EventDefinitionRecord : IRecord
    {
        public string Id { get; set; }
        public string Vpn { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string JsonSchema { get; set; }
        public string MessageTopic { get; set; }
        public ICollection<EventDefinitionLink> Links { get; set; } = new List<EventDefinitionLink>();

        public bool Handle(RemoveLinkEventDefinitionCommand cmd)
        {
            var link = Links.SingleOrDefault(l => l.Source == cmd.Source && l.Target == cmd.Target);
            if (link == null) return false;
            Links.Remove(link);
            return true;
        }

        public bool Handle(AddLinkEventDefinitionCommand cmd)
        {
            var link = Links.SingleOrDefault(l => l.Source == cmd.Source && l.Target == cmd.Target);
            if (link != null) return false;
            Links.Add(new EventDefinitionLink { Source = cmd.Source, Target = cmd.Target });
            return true;
        }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Vpn = context.NextString();
            Topic = context.NextString();
            Description = context.NextString();
            CreateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks);
            UpdateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks);
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
            context.WriteString(Topic);
            context.WriteString(Description);
            context.WriteTimeSpan(TimeSpan.FromTicks(CreateDateTime.Ticks));
            context.WriteTimeSpan(TimeSpan.FromTicks(UpdateDateTime.Ticks));
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
