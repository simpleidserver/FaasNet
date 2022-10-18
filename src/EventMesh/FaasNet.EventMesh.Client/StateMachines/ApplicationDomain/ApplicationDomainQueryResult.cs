using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.StateMachines.ApplicationDomain
{
    public class ApplicationDomainQueryResult : ISerializable
    {
        public string Name { get; set; }
        public string Vpn { get; set; }
        public string Description { get; set; }
        public string RootTopic { get; set; }
        public ICollection<ApplicationDomainElementResult> Elements { get; set; } = new List<ApplicationDomainElementResult>();
        public DateTime? CreateDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Name = context.NextString();
            Vpn = context.NextString();
            Description = context.NextString();
            RootTopic = context.NextString();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++)
            {
                var elt = new ApplicationDomainElementResult();
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
            context.WriteInteger(Elements.Count());
            foreach (var elt in Elements) elt.Serialize(context);
            context.WriteTimeSpan(TimeSpan.FromTicks(CreateDateTime.Value.Ticks));
            context.WriteTimeSpan(TimeSpan.FromTicks(UpdateDateTime.Value.Ticks));
        }
    }

    public class ApplicationDomainElementResult : ISerializable
    {
        public string ElementId { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public ICollection<ApplicationDomainElementLinkResult> Targets { get; set; } = new List<ApplicationDomainElementLinkResult>();

        public void Deserialize(ReadBufferContext context)
        {
            ElementId = context.NextString();
            CoordinateX = context.NextDouble();
            CoordinateY = context.NextDouble();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++)
            {
                var link = new ApplicationDomainElementLinkResult();
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

    public class ApplicationDomainElementLinkResult : ISerializable
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
