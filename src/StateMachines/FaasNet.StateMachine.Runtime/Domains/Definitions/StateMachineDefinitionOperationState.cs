using FaasNet.StateMachine.Runtime.Domains.Enums;
using System;
using System.Collections.Generic;

namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public class StateMachineDefinitionOperationState : BaseStateMachineFlowableState
    {
        public StateMachineDefinitionOperationState()
        {
            Actions = new List<StateMachineDefinitionAction>();
        }

        /// <summary>
        /// Should actions be performed sequentially or in parallel.
        /// </summary>
        public StateMachineDefinitionActionModes ActionMode { get; set; }
        /// <summary>
        /// Actions to be performed.
        /// </summary>
        public virtual ICollection<StateMachineDefinitionAction> Actions { get; set; }

        public static StateMachineDefinitionOperationState Create()
        {
            return new StateMachineDefinitionOperationState
            {
                Id = Guid.NewGuid().ToString(),
                Type = StateMachineDefinitionStateTypes.Operation
            };
        }
    }
}
