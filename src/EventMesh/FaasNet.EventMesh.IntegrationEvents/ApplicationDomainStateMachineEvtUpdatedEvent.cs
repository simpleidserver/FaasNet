using FaasNet.Domain;
using System.Collections.Generic;

namespace FaasNet.EventMesh.IntegrationEvents
{
    public class ApplicationDomainStateMachineEvtUpdatedEvent : IntegrationEvent
    {
        public ApplicationDomainStateMachineEvtUpdatedEvent(string id, string aggregateId, ICollection<StateMachineEvt> evts) : base(id, aggregateId)
        {
            Evts = evts;
        }

        public ICollection<StateMachineEvt> Evts { get; set; }
    }

    public class StateMachineEvt
    {
        public string MessageId { get; set; }
        public string TopicName { get; set; }
        public bool IsConsumed { get; set; }
    }
}
