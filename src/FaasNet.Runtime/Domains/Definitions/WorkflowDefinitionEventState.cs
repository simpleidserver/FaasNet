using FaasNet.Runtime.Domains.Enums;
using System;
using System.Collections.Generic;

namespace FaasNet.Runtime.Domains.Definitions
{
    public class WorkflowDefinitionEventState : BaseWorkflowDefinitionFlowableState
    {
        public WorkflowDefinitionEventState()
        {
            Exclusive = true;
            OnEvents = new List<WorkflowDefinitionOnEvent>();
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
        public virtual ICollection<WorkflowDefinitionOnEvent> OnEvents { get; set; }

        public static WorkflowDefinitionEventState Create()
        {
            return new WorkflowDefinitionEventState
            {
                Id = Guid.NewGuid().ToString(),
                Type = WorkflowDefinitionStateTypes.Event,
                Exclusive = true
            };
        }
    }
}
