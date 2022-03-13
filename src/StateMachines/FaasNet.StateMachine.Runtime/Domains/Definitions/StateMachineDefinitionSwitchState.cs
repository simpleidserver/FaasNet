using FaasNet.StateMachine.Runtime.Domains.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public class StateMachineDefinitionSwitchState : BaseStateMachineDefinitionState
    {
        public StateMachineDefinitionSwitchState()
        {
            Conditions = new List<BaseEventCondition>();
            Type = Enums.StateMachineDefinitionStateTypes.Switch;
        }

        /// <summary>
        /// Defined if the Switch state evaluates conditions and transitions based on state data, or arrival of events.
        /// </summary>
        [JsonIgnore]
        [YamlIgnore]
        public virtual ICollection<BaseEventCondition> Conditions { get; set; }
        /// <summary>
        /// Default transition of the workflow if there is no matching data conditions or event timeout is reached. 
        /// </summary>
        [JsonIgnore]
        [YamlIgnore]
        public string DefaultConditionStr { get; set; }
        public ICollection<StateMachineDefinitionSwitchDataCondition> DataConditions
        {
            get
            {
                return Conditions.Where(c => c.ConditionType == StateMachineDefinitionEventConditionTypes.DATA).Cast<StateMachineDefinitionSwitchDataCondition>().ToList();
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
        public ICollection<StateMachineDefinitionSwitchEventCondition> EventConditions
        {
            get
            {
                return Conditions.Where(c => c.ConditionType == StateMachineDefinitionEventConditionTypes.EVENT).Cast<StateMachineDefinitionSwitchEventCondition>().ToList();
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
        public StateMachineDefinitionDefaultCondition DefaultCondition
        {
            get
            {
                return string.IsNullOrWhiteSpace(DefaultConditionStr) ? null : new StateMachineDefinitionDefaultCondition
                {
                    Transition = DefaultConditionStr
                };
            }
            set
            {
                if (value != null)
                {
                    DefaultConditionStr = value.Transition;
                }
            }
        }


        public static StateMachineDefinitionSwitchState Create()
        {
            return new StateMachineDefinitionSwitchState
            {
                Id = Guid.NewGuid().ToString(),
                Type = StateMachineDefinitionStateTypes.Switch
            };
        }
    }
}
