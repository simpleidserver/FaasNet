using FaasNet.Application.Core.Domains.DomainEvents;
using FaasNet.Domain;
using System;

namespace FaasNet.Application.Core.Domains
{
    public class ApplicationDomainAggregate : AggregateRoot
    {
        public static string TOPIC_NAME = "applications";

        private ApplicationDomainAggregate()
        {

        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string RootTopic { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public static ApplicationDomainAggregate Create(string name, string description, string rootTopic)
        {
            var id = Guid.NewGuid().ToString();
            var result = new ApplicationDomainAggregate();
            var evt = new ApplicationDomainCreatedEvent(Guid.NewGuid().ToString(), id, name, description, rootTopic, DateTime.UtcNow);
            result.DomainEvts.Add(evt);
            result.Handle(evt);
            return result;
        }

        public override string Topic => TOPIC_NAME;

        public override void Handle(dynamic evt)
        {
            Handle(evt);
        }

        public void Handle(ApplicationDomainCreatedEvent evt)
        {
            Id = evt.AggregateId;
            Version = evt.AggregateVersion;
            Name = evt.Name;
            Description = evt.Description;
            RootTopic = evt.RootTopic;
            CreateDateTime = evt.CreateDateTime;
            UpdateDateTime = evt.CreateDateTime;
        }
    }
}
