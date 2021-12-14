using FaasNet.Runtime.Domains.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

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
        [JsonIgnore]
        [YamlIgnore]
        public virtual ICollection<BaseEventCondition> Conditions { get; set; }
        public ICollection<WorkflowDefinitionSwitchDataCondition> DataConditions
        {
            get
            {
                return Conditions.Where(c => c.ConditionType == WorkflowDefinitionEventConditionTypes.DATA).Cast<WorkflowDefinitionSwitchDataCondition>().ToList();
            }
            set
            {
                if(value != null)
                {
                    foreach (var record in value) 
                    {
                        Conditions.Add(record);
                    }
                }
            }
        }
        public ICollection<WorkflowDefinitionSwitchEventCondition> EventConditions
        {
            get
            {
                return Conditions.Where(c => c.ConditionType == WorkflowDefinitionEventConditionTypes.EVENT).Cast<WorkflowDefinitionSwitchEventCondition>().ToList();
            }
            set
            {
                if (value != null)
                {
                    foreach (var record in value)
                    {
                        Conditions.Add(record);
                    }
                }
            }
        }

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
