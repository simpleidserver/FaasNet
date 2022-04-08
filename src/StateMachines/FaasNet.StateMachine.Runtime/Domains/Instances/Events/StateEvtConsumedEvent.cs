using FaasNet.Domain;
using System.Diagnostics;

namespace FaasNet.StateMachine.Runtime.Domains.Instances.Events
{
    [DebuggerDisplay("State {StateId} consume event '{EvtType}'")]
    public class StateEvtConsumedEvent : DomainEvent
    {
        public StateEvtConsumedEvent(string id, string aggregateId, string stateId, string evtSource, string evtType, string input) : base(id, aggregateId)
        {
            StateId = stateId;
            EvtSource = evtSource;
            EvtType = evtType;
            Input = input;
        }

        public string StateId {  get; set; }
        public string EvtSource { get; set; }
        public string EvtType { get; set; }
        public string Input { get; set; }
    }
}
