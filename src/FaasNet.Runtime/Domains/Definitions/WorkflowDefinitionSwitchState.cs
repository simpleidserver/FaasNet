using FaasNet.Runtime.Domains.Enums;
using System;
using System.Collections.Generic;

namespace FaasNet.Runtime.Domains.Definitions
{
    public class WorkflowDefinitionSwitchState : BaseWorkflowDefinitionState
    {
        public WorkflowDefinitionSwitchState()
        {
            Conditions = new List<BaseEventCondition>();
            Type = Enums.WorkflowDefinitionStateTypes.Switch;
        }

        /// <summary>
        /// Defined if the Switch state evaluates conditions and transitions based on state data, or arrival of events.
        /// </summary>
        public ICollection<BaseEventCondition> Conditions { get; set; }

        public static WorkflowDefinitionSwitchState Create()
        {
            return new WorkflowDefinitionSwitchState
            {
                Id = Guid.NewGuid().ToString(),
                Type = WorkflowDefinitionStateTypes.Switch
            };
        }
    }
}
