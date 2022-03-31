using FaasNet.Domain;
using System;

namespace FaasNet.StateMachine.IntegrationEvents
{
    public class StateMachineDefinitionAddedEvent : IntegrationEvent
    {
        public StateMachineDefinitionAddedEvent(string aggregateId, string name, string description, string vpn) : base(Guid.NewGuid().ToString(), aggregateId)
        {
            Vpn = vpn;
            Name = name;
            Description = description;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Vpn { get; set; }
    }
}
