using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace FaasNet.Runtime.Domains.Definitions
{
    public class WorkflowDefinitionAggregate
    {
        public WorkflowDefinitionAggregate()
        {
            States = new List<BaseWorkflowDefinitionState>();
            Functions = new List<WorkflowDefinitionFunction>();
            Events = new List<WorkflowDefinitionEvent>();
        }

        #region Properties

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
        [JsonIgnore]
        [YamlIgnore]
        public DateTime CreateDateTime { get; set; }
        [JsonIgnore]
        [YamlIgnore]
        public DateTime UpdateDateTime { get; set; }
        /// <summary>
        /// Workflow start definition.
        /// </summary>
        public virtual WorkflowDefinitionStartState Start { get; set; }
        /// <summary>
        /// Workflow states.
        /// </summary>
        public virtual ICollection<BaseWorkflowDefinitionState> States { get; set; }
        /// <summary>
        /// Workflow function definitions.
        /// </summary>
        public virtual ICollection<WorkflowDefinitionFunction> Functions { get; set; }
        /// <summary>
        /// Workflow event definitions.
        /// </summary>
        public virtual ICollection<WorkflowDefinitionEvent> Events { get; set; }

        #endregion

        public static WorkflowDefinitionAggregate CreateEmpty(string id, string name, string description)
        {
            var result = Create(id, "1.0", name, description);
            result.States.Add(new WorkflowDefinitionInjectState
            {
                Id = Guid.NewGuid().ToString(),
                Name = "helloWorld",
                DataStr = "{'message' : 'Hello World !' }",
                IsRootState = true
            });
            return result;
        }

        public static WorkflowDefinitionAggregate Create(string id, string version, string name, string description)
        {
            return new WorkflowDefinitionAggregate
            {
                Id = id,
                Version = version,
                Name = name,
                Description = description,
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow
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

        public BaseWorkflowDefinitionState GetStateByName(string name)
        {
            return States.FirstOrDefault(s => s.Name == name);
        }

        public WorkflowDefinitionEvent GetEvent(string name)
        {
            return Events.FirstOrDefault(e => e.Name == name);
        }

        public BaseWorkflowDefinitionState GetRootState()
        {
            return States.First(s => s.IsRootState);
        }

        public void UpdateStates(ICollection<BaseWorkflowDefinitionState> states)
        {
            States.Clear();
            foreach(var s in states)
            {
                States.Add(s);
            }
        }

        public void UpdateFunctions(ICollection<WorkflowDefinitionFunction> functions)
        {
            Functions.Clear();
            foreach(var f in functions)
            {
                Functions.Add(f);
            }
        }

        public void UpdateEvents(ICollection<WorkflowDefinitionEvent> events)
        {
            Events.Clear();
            foreach(var e in events)
            {
                Events.Add(e);
            }
        }
    }
}
