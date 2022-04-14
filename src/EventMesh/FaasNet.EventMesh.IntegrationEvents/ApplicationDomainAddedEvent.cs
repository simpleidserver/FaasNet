using FaasNet.Domain;
using System;

namespace FaasNet.EventMesh.IntegrationEvents
{
    public class ApplicationDomainAddedEvent : IntegrationEvent
    {
        public ApplicationDomainAddedEvent(string aggregateId, string rootTopic) : base(Guid.NewGuid().ToString(), aggregateId)
        {
            RootTopic = rootTopic;
        }

        public string RootTopic { get; set; }
    }
}
