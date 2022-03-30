using FaasNet.Domain;
using System;

namespace FaasNet.StateMachine.IntegrationEvents
{
    public class StateMachineDefinitionAddedEvent : IntegrationEvent
    {
        public StateMachineDefinitionAddedEvent(string aggregateId, string vpn) : base(Guid.NewGuid().ToString(), aggregateId)
        {
            Vpn = vpn;
        }

        public string Vpn { get; set; }
    }
}
