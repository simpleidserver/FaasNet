using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
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

namespace FaasNet.EventMesh.StateMachines.ApplicationDomain
{
    public class ApplicationDomainStateMachine : BaseStateMachine
    {
        private readonly IApplicationDomainStateMachineStore _store;

        public ApplicationDomainStateMachine(IApplicationDomainStateMachineStore store, IPeerInfoStore peerInfoStore, IMediator mediator) : base(peerInfoStore, mediator)
        {
            _store = store;
        }

        public override async Task Apply(ICommand cmd, CancellationToken cancellationToken)
        {
            switch(cmd)
            {
                case AddApplicationDomainCommand addApplicationDomainCommand:
                    {
                        var existingApplicationDomain = await _store.Get(addApplicationDomainCommand.Name, addApplicationDomainCommand.Vpn, cancellationToken);
                        if (existingApplicationDomain != null) return;
                        var record = new ApplicationDomainRecord
                        {
                            Name = addApplicationDomainCommand.Name,
                            Description = addApplicationDomainCommand.Description,
                            RootTopic = addApplicationDomainCommand.RootTopic,
                            Vpn = addApplicationDomainCommand.Vpn,
                            CreateDateTime = DateTime.UtcNow,
                            UpdateDateTime = DateTime.UtcNow
                        };
                        _store.Add(record);
                        await _store.SaveChanges(cancellationToken);
                    }
                    break;
                case UpdateApplicationDomainCoordinatesCommand updateApplicationDomainCoordinatesCommand:
                    {
                        var existingApplicationDomain = await _store.Get(updateApplicationDomainCoordinatesCommand.Name, updateApplicationDomainCoordinatesCommand.Vpn, cancellationToken);
                        if (existingApplicationDomain == null) return;
                        if(existingApplicationDomain.Handle(updateApplicationDomainCoordinatesCommand))
                        {
                            _store.Update(existingApplicationDomain);
                            await _store.SaveChanges(cancellationToken);
                        }
                    }
                    break;
                case AddApplicationDomainElementCommand addApplicationDomainElementCommand:
                    {
                        var existingApplicationDomain = await _store.Get(addApplicationDomainElementCommand.Name, addApplicationDomainElementCommand.Vpn, cancellationToken);
                        if (existingApplicationDomain == null) return;
                        if (existingApplicationDomain.Handle(addApplicationDomainElementCommand))
                        {
                            _store.Update(existingApplicationDomain);
                            await _store.SaveChanges(cancellationToken);
                        }
                    }

                    break;
                case AddApplicationDomainLinkCommand addApplicationDomainLinkCommand:
                    {
                        var existingApplicationDomain = await _store.Get(addApplicationDomainLinkCommand.Name, addApplicationDomainLinkCommand.Vpn, cancellationToken);
                        if (existingApplicationDomain == null) return;
                        if (existingApplicationDomain.Handle(addApplicationDomainLinkCommand))
                        {
                            _store.Update(existingApplicationDomain);
                            await _store.SaveChanges(cancellationToken);
                            await PropagateIntegrationEvent(new ApplicationDomainLinkAdded
                            {
                                Source = addApplicationDomainLinkCommand.Source,
                                Target = addApplicationDomainLinkCommand.Target,
                                EventId = addApplicationDomainLinkCommand.EventId,
                                Vpn = addApplicationDomainLinkCommand.Vpn
                            }, cancellationToken);
                        }
                    }
                    break;
                case RemoveApplicationDomainLinkCommand removeApplicationDomainLinkCommand:
                    {
                        var existingApplicationDomain = await _store.Get(removeApplicationDomainLinkCommand.Name, removeApplicationDomainLinkCommand.Vpn, cancellationToken);
                        if (existingApplicationDomain == null) return;
                        if (existingApplicationDomain.Handle(removeApplicationDomainLinkCommand))
                        {
                            _store.Update(existingApplicationDomain);
                            await _store.SaveChanges(cancellationToken);
                            await PropagateIntegrationEvent(new ApplicationDomainLinkRemoved
                            {
                                Source = removeApplicationDomainLinkCommand.Source,
                                Target = removeApplicationDomainLinkCommand.Target,
                                EventId = removeApplicationDomainLinkCommand.EventId,
                                Vpn = removeApplicationDomainLinkCommand.Vpn
                            }, cancellationToken);
                        }
                    }
                    break;
            }
        }

        public override Task BulkUpload(IEnumerable<IRecord> records, CancellationToken cancellationToken)
        {
            return _store.BulkUpload(records.Cast<ApplicationDomainRecord>(), cancellationToken);
        }

        public override Task Commit(CancellationToken cancellationToken)
        {
            return _store.SaveChanges(cancellationToken);
        }

        public override async Task<IQueryResult> Query(IQuery query, CancellationToken cancellationToken)
        {
            switch(query)
            {
                case GetAllApplicationDomainsQuery getAllApplicationDomains:
                    var res = await _store.Find(getAllApplicationDomains.Filter, cancellationToken);
                    return new GenericSearchQueryResult<ApplicationDomainQueryResult>
                    {
                        TotalPages = res.TotalPages,
                        TotalRecords = res.TotalRecords,
                        Records = res.Records.Select(r => Transform(r))
                    };
            }

            return null;
        }

        public override IEnumerable<(IEnumerable<IEnumerable<byte>>, int)> Snapshot(int nbRecords)
        {
            var name = typeof(ApplicationDomainRecord).AssemblyQualifiedName;
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

        private static ApplicationDomainQueryResult Transform(ApplicationDomainRecord applicationDomain)
        {
            return new ApplicationDomainQueryResult
            {
                CreateDateTime = applicationDomain.CreateDateTime,
                Description = applicationDomain.Description,
                Name = applicationDomain.Name,
                RootTopic = applicationDomain.RootTopic,
                UpdateDateTime = applicationDomain.UpdateDateTime,
                Vpn = applicationDomain.Vpn
            };
        }
    }

    public class ApplicationDomainRecord : IRecord
    {
        public string Name { get; set; }
        public string Vpn { get; set; }
        public string Description { get; set; }
        public string RootTopic { get; set; }
        public ICollection<ApplicationDomainElement> Elements { get; set; } = new List<ApplicationDomainElement>();
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public bool Handle(AddApplicationDomainElementCommand cmd)
        {
            var element = Elements.SingleOrDefault(e => e.ElementId == cmd.ElementId);
            if (element != null) return false;
            Elements.Add(new ApplicationDomainElement
            {
                ElementId = cmd.ElementId,
                CoordinateX = cmd.CoordinateX,
                CoordinateY = cmd.CoordinateY
            });
            return true;
        }

        public bool Handle(UpdateApplicationDomainCoordinatesCommand cmd)
        {
            foreach(var elt in Elements)
            {
                var sourceElt = cmd.Elements.FirstOrDefault(e => e.ElementId == elt.ElementId);
                if (sourceElt == null) continue;
                elt.CoordinateX = sourceElt.CoordinateX;
                elt.CoordinateY = sourceElt.CoordinateY;
            }

            return true;
        }

        public bool Handle(AddApplicationDomainLinkCommand cmd)
        {
            var element = Elements.SingleOrDefault(e => e.ElementId == cmd.Source);
            if (element == null) return false;
            var target = element.Targets.SingleOrDefault(t => t.Target == cmd.Target);
            if (target != null) return false;
            element.Targets.Add(new ApplicationDomainElementLink
            {
                EventId = cmd.EventId,
                Target = cmd.Target
            });
            return true;
        }

        public bool Handle(RemoveApplicationDomainLinkCommand cmd)
        {
            var element = Elements.SingleOrDefault(e => e.ElementId == cmd.Source);
            if (element == null) return false;
            var target = element.Targets.SingleOrDefault(t => t.Target == cmd.Target);
            if (target == null) return false;
            element.Targets.Remove(target);
            return true;
        }

        public void Deserialize(ReadBufferContext context)
        {
            Name = context.NextString();
            Vpn = context.NextString();
            Description = context.NextString();
            RootTopic = context.NextString();
            var nb = context.NextInt();
            for(var i = 0; i < nb; i++)
            {
                var elt = new ApplicationDomainElement();
                elt.Deserialize(context);
                Elements.Add(elt);
            }

            CreateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks);
            UpdateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks);
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Vpn);
            context.WriteString(Description);
            context.WriteString(RootTopic);
            foreach (var elt in Elements) elt.Serialize(context);
            context.WriteTimeSpan(TimeSpan.FromTicks(CreateDateTime.Ticks));
            context.WriteTimeSpan(TimeSpan.FromTicks(UpdateDateTime.Ticks));
        }
    }

    public class ApplicationDomainElement : ISerializable
    {
        public string ElementId { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public ICollection<ApplicationDomainElementLink> Targets { get; set; } = new List<ApplicationDomainElementLink>();

        public void Deserialize(ReadBufferContext context)
        {
            ElementId = context.NextString();
            CoordinateX = context.NextDouble();
            CoordinateY = context.NextDouble();
            var nb = context.NextInt();
            for(var i = 0; i < nb; i++)
            {
                var link = new ApplicationDomainElementLink();
                link.Deserialize(context);
                Targets.Add(link);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(ElementId);
            context.WriteDouble(CoordinateX);
            context.WriteDouble(CoordinateY);
            foreach (var target in Targets) target.Serialize(context);
        }
    }

    public class ApplicationDomainElementLink : ISerializable
    {
        public string EventId { get; set; }
        public string Target { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            EventId = context.NextString();
            Target = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(EventId);
            context.WriteString(Target);
        }
    }
}
