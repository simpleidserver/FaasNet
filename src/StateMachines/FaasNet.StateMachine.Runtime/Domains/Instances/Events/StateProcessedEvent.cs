using FaasNet.Domain;
using System.Diagnostics;

namespace FaasNet.StateMachine.Runtime.Domains.Instances.Events
{
    [DebuggerDisplay("State {StateId} process event '{EvtName}'")]
    public class StateProcessedEvent : DomainEvent
    {
        public StateProcessedEvent(string id, string aggregateId, string stateId, string evtName, string output) : base(id, aggregateId)
        {
            StateId = stateId;
            EvtName = evtName;
            Output = output;
        }

        public string StateId { get; set; }
        public string EvtName { get; set; }
        public string Output { get; set; }
    }
}
