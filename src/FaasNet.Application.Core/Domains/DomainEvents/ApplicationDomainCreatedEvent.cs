using FaasNet.Domain;
using System;

namespace FaasNet.Application.Core.Domains.DomainEvents
{
    public class ApplicationDomainCreatedEvent : DomainEvent
    {
        public ApplicationDomainCreatedEvent(string id, string aggregateId, string name, DateTime createDateTime) : base(id, aggregateId)
        {
            Name = name;
            CreateDateTime = createDateTime;
        }

        public string Name { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
