using FaasNet.StateMachine.Runtime.Domains.Enums;
using System;

namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public class StateMachineDefinitionEvent
    {
        public StateMachineDefinitionEvent()
        {
            Kind = StateMachineDefinitionEventKinds.Consumed;
        }

        /// <summary>
        /// Unique event name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// CloudEvent source.
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// CloudEvent type.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Defines the event is either consumed or produced by the workflow. 
        /// Default is consumed
        /// </summary>
        public StateMachineDefinitionEventKinds Kind { get; set; }
    }
}
