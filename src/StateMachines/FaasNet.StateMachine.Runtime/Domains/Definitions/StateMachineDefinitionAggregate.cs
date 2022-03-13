using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public class StateMachineDefinitionAggregate
    {
        public StateMachineDefinitionAggregate()
        {
            States = new List<BaseStateMachineDefinitionState>();
            Functions = new List<StateMachineDefinitionFunction>();
            Events = new List<StateMachineDefinitionEvent>();
        }

        #region Properties

        [JsonIgnore]
        [YamlIgnore]
        /// <summary>
        /// Technical Identifier.
        /// </summary>
        public string TechnicalId { get; set; }
        /// <summary>
        /// Is Last.
        /// </summary>
        public bool IsLast { get; set; }
        /// <summary>
        /// Workflow unique identifier.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Workflow version.
        /// </summary>
        public int Version { get; set; }
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
        public virtual StateMachineDefinitionStartState Start { get; set; }
        /// <summary>
        /// Workflow states.
        /// </summary>
        public virtual ICollection<BaseStateMachineDefinitionState> States { get; set; }
        /// <summary>
        /// Workflow function definitions.
        /// </summary>
        public virtual ICollection<StateMachineDefinitionFunction> Functions { get; set; }
        /// <summary>
        /// Workflow event definitions.
        /// </summary>
        public virtual ICollection<StateMachineDefinitionEvent> Events { get; set; }

        #endregion

        public static StateMachineDefinitionAggregate CreateEmpty(string id, string name, string description)
        {
            var result = Create(id, 1, name, description);
            var stateId = Guid.NewGuid().ToString();
            result.States.Add(new StateMachineDefinitionInjectState
            {
                Id = stateId,
                Name = "helloWorld",
                DataStr = "{'message' : 'Hello World !' }",
                End = true
            });
            result.Start = new StateMachineDefinitionStartState
            {
                StateName = stateId
            };
            result.RefreshTechnicalId();
            return result;
        }

        public static StateMachineDefinitionAggregate Create(string id, int version, string name, string description)
        {
            var result = new StateMachineDefinitionAggregate
            {
                Id = id,
                Version = version,
                Name = name,
                Description = description,
                IsLast = true,
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow
            };
            result.RefreshTechnicalId();
            return result;
        }

        public StateMachineDefinitionFunction GetFunction(string name)
        {
            return Functions.FirstOrDefault(f => f.Name == name);
        }

        public BaseStateMachineDefinitionState GetState(string id)
        {
            return States.FirstOrDefault(s => s.Id == id);
        }

        public StateMachineDefinitionEvent GetEvent(string name)
        {
            return Events.FirstOrDefault(e => e.Name == name);
        }

        public BaseStateMachineDefinitionState GetRootState()
        {
            return States.First(s => s.Id == Start.StateName);
        }

        public void RefreshTechnicalId()
        {
            TechnicalId = Guid.NewGuid().ToString();
            foreach(var state in States)
            {
                state.TechnicalId = Guid.NewGuid().ToString();
            }
        }
    }
}
