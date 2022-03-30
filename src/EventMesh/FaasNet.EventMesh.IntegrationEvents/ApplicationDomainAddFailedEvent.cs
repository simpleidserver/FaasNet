using FaasNet.Domain;
using System;

namespace FaasNet.EventMesh.IntegrationEvents
{
    public class ApplicationDomainAddFailedEvent : IntegrationEvent
    {
        public ApplicationDomainAddFailedEvent(string aggregateId) : base(Guid.NewGuid().ToString(), aggregateId)
        {
        }
    }
}
