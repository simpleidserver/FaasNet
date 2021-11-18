using FaasNet.Runtime.Domains.Enums;
using System;
using System.Collections.Generic;

namespace FaasNet.Runtime.Domains.Definitions
{
    public class WorkflowDefinitionForeachState : BaseWorkflowDefinitionFlowableState
    {
        public WorkflowDefinitionForeachState()
        {
            Type = Enums.WorkflowDefinitionStateTypes.ForEach;
            Mode = WorkflowDefinitionForeachStateModes.Parallel;
            Actions = new List<WorkflowDefinitionAction>();
        }

        /// <summary>
        /// Workflow expression selecting an array element of the states data.
        /// </summary>
        public string InputCollection { get; set; }
        /// <summary>
        /// Workflow expression specifying an array element of the states data to add the results of each iteration.
        /// </summary>
        public string OutputCollection { get; set; }
        /// <summary>
        /// Name of the iteration parameter that can be referenced in actions/workflow.
        /// For each parallel iteration, this param should contain an unique element of the inputCollection array.
        /// </summary>
        public string IterationParam { get; set; }
        /// <summary>
        /// Specifies how many iterations may run in parallel at the same time. 
        /// </summary>
        public int? BatchSize { get; set; }
        /// <summary>
        /// Specifies how iterations are to be performed (sequentially or in parallel). Default is parallel.
        /// </summary>
        public WorkflowDefinitionForeachStateModes Mode { get; set; }
        /// <summary>
        /// Actions to be executed for each of the elements of inputCollection.
        /// </summary>
        public virtual ICollection<WorkflowDefinitionAction> Actions { get; set; }

        public static WorkflowDefinitionForeachState Create()
        {
            return new WorkflowDefinitionForeachState
            {
                Id = Guid.NewGuid().ToString(),
                Type = WorkflowDefinitionStateTypes.ForEach
            };
        }
    }
}
