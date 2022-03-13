using FaasNet.Domain;
using System;

namespace FaasNet.Application.Core.Domains.DomainEvents
{
    public class ApplicationDomainCreatedEvent : DomainEvent
    {
        public ApplicationDomainCreatedEvent(string id, string aggregateId, string name, string description, string rootTopic, DateTime createDateTime) : base(id, aggregateId)
        {
            Name = name;
            Description = description;
            RootTopic = rootTopic;
            CreateDateTime = createDateTime;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string RootTopic { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
