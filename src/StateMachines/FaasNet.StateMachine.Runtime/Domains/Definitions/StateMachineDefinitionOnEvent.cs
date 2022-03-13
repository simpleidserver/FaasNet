using FaasNet.StateMachine.Runtime.Domains.Enums;
using System.Collections.Generic;

namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public class StateMachineDefinitionOnEvent
    {
        public StateMachineDefinitionOnEvent()
        {
            EventRefs = new List<string>();
            ActionMode = StateMachineDefinitionActionModes.Sequential;
            Actions = new List<StateMachineDefinitionAction>();
        }

        /// <summary>
        /// References one or more unique event names in the defined workflow events.
        /// </summary>
        public ICollection<string> EventRefs { get; set; }
        /// <summary>
        /// Specifies how actions are to be performed (in sequence or in parallel). 
        /// Default is "sequential"
        /// </summary>
        public StateMachineDefinitionActionModes ActionMode { get; set; }
        /// <summary>
        /// Actions to be performed.
        /// </summary>
        public virtual ICollection<StateMachineDefinitionAction> Actions { get; set; }
        /// <summary>
        /// Event data filter definition.
        /// </summary>
        public virtual StateMachineDefinitionEventDataFilter EventDataFilter { get; set; }
    }
}
