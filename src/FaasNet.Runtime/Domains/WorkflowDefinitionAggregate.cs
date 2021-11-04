﻿using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Runtime.Domains
{
    public class WorkflowDefinitionAggregate
    {
        public WorkflowDefinitionAggregate()
        {
            States = new List<BaseWorkflowDefinitionState>();
            Functions = new List<WorkflowDefinitionFunction>();
            Events = new List<WorkflowDefinitionEvent>();
        }

        /// <summary>
        /// Workflow unique identifier.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Workflow version.
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Worfklow name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Workflow description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Workflow start definition.
        /// </summary>
        public WorkflowDefinitionStartState Start { get; set; }
        /// <summary>
        /// Workflow states.
        /// </summary>
        public ICollection<BaseWorkflowDefinitionState> States { get; set; }
        /// <summary>
        /// Workflow function definitions.
        /// </summary>
        public ICollection<WorkflowDefinitionFunction> Functions { get; set; }
        /// <summary>
        /// Workflow event definitions.
        /// </summary>
        public ICollection<WorkflowDefinitionEvent> Events { get; set; }

        public static WorkflowDefinitionAggregate Create(string id, string version, string name, string description)
        {
            return new WorkflowDefinitionAggregate
            {
                Id = id,
                Version = version,
                Name = name,
                Description = description
            };
        }

        public WorkflowDefinitionFunction GetFunction(string name)
        {
            return Functions.FirstOrDefault(f => f.Name == name);
        }

        public BaseWorkflowDefinitionState GetState(string id)
        {
            return States.FirstOrDefault(s => s.Id == id);
        }

        public WorkflowDefinitionEvent GetEvent(string name)
        {
            return Events.FirstOrDefault(e => e.Name == name);
        }
    }
}