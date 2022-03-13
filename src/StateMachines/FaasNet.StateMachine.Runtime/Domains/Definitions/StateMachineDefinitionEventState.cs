using FaasNet.StateMachine.Runtime.Domains.Enums;
using System;
using System.Collections.Generic;

namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public class StateMachineDefinitionEventState : BaseStateMachineFlowableState
    {
        public StateMachineDefinitionEventState()
        {
            Exclusive = true;
            OnEvents = new List<StateMachineDefinitionOnEvent>();
        }

        /// <summary>
        /// If "true", consuming one of the defined events causes its associated actions to be performed. 
        /// If "false", all of the defined events must be consumed in order for actions to be performed. 
        /// Default is "true"
        /// </summary>
        public bool Exclusive { get; set; }
        /// <summary>
        /// Define the events to be consumed and optional actions to be performed.
        /// </summary>
        public virtual ICollection<StateMachineDefinitionOnEvent> OnEvents { get; set; }

        public static StateMachineDefinitionEventState Create()
        {
            return new StateMachineDefinitionEventState
            {
                Id = Guid.NewGuid().ToString(),
                Type = StateMachineDefinitionStateTypes.Event,
                Exclusive = true
            };
        }
    }
}
