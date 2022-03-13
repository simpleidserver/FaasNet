using FaasNet.StateMachine.Runtime.Domains.Enums;
using System;
using System.Collections.Generic;

namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public class StateMachineDefinitionForeachState : BaseStateMachineFlowableState
    {
        public StateMachineDefinitionForeachState()
        {
            Type = Enums.StateMachineDefinitionStateTypes.ForEach;
            Mode = StateMachineDefinitionForeachStateModes.Parallel;
            Actions = new List<StateMachineDefinitionAction>();
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
        public StateMachineDefinitionForeachStateModes Mode { get; set; }
        /// <summary>
        /// Actions to be executed for each of the elements of inputCollection.
        /// </summary>
        public virtual ICollection<StateMachineDefinitionAction> Actions { get; set; }

        public static StateMachineDefinitionForeachState Create()
        {
            return new StateMachineDefinitionForeachState
            {
                Id = Guid.NewGuid().ToString(),
                Type = StateMachineDefinitionStateTypes.ForEach
            };
        }
    }
}
