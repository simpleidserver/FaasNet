using FaasNet.Domain;
using System.Diagnostics;

namespace FaasNet.StateMachine.Runtime.Domains.Instances.Events
{
    [DebuggerDisplay("State instance '{StateInstanceId}' is created")]
    public class StateInstanceCreatedEvent : DomainEvent
    {
        public StateInstanceCreatedEvent(string id, string aggregateId, string stateInstanceId, string defId) : base(id, aggregateId)
        {
            StateInstanceId = stateInstanceId;
            DefId = defId;
        }

        public string StateInstanceId { get; set; }
        public string DefId { get; set; }
    }
}
