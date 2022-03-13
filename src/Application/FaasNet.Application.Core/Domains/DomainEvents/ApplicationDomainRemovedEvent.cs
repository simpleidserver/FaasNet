using FaasNet.Domain;

namespace FaasNet.Application.Core.Domains.DomainEvents
{
    public class ApplicationDomainRemovedEvent : DomainEvent
    {
        public ApplicationDomainRemovedEvent(string id, string aggregateId) : base(id, aggregateId)
        {
        }
    }
}
