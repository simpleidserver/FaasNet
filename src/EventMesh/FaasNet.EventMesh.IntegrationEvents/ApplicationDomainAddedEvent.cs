using FaasNet.Domain;
using System;

namespace FaasNet.EventMesh.IntegrationEvents
{
    public class ApplicationDomainAddedEvent : IntegrationEvent
    {
        public ApplicationDomainAddedEvent(string aggregateId) : base(Guid.NewGuid().ToString(), aggregateId)
        {
        }
    }
}
