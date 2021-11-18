using FaasNet.Runtime.Domains.Enums;
using System;
using System.Collections.Generic;

namespace FaasNet.Runtime.Domains.Definitions
{
    public class WorkflowDefinitionOperationState : BaseWorkflowDefinitionFlowableState
    {
        public WorkflowDefinitionOperationState()
        {
            Actions = new List<WorkflowDefinitionAction>();
        }

        /// <summary>
        /// Should actions be performed sequentially or in parallel.
        /// </summary>
        public WorkflowDefinitionActionModes ActionMode { get; set; }
        /// <summary>
        /// Actions to be performed.
        /// </summary>
        public virtual ICollection<WorkflowDefinitionAction> Actions { get; set; }

        public static WorkflowDefinitionOperationState Create()
        {
            return new WorkflowDefinitionOperationState
            {
                Id = Guid.NewGuid().ToString(),
                Type = WorkflowDefinitionStateTypes.Operation
            };
        }
    }
}
