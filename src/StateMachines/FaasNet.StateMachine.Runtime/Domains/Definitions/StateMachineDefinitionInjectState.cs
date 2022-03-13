using FaasNet.StateMachine.Runtime.Domains.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using YamlDotNet.Serialization;

namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public class StateMachineDefinitionInjectState: BaseStateMachineFlowableState
    {
        public StateMachineDefinitionInjectState()
        {
            Type = StateMachineDefinitionStateTypes.Inject;
        }

        #region Properties

        [YamlIgnore]
        [JsonIgnore]
        public string DataStr { get; set; }
        public JObject Data
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DataStr))
                {
                    return null;
                }

                return JObject.Parse(DataStr);
            }
            set
            {
                DataStr = value.ToString();
            }
        }

        #endregion

        public static StateMachineDefinitionInjectState Create()
        {
            return new StateMachineDefinitionInjectState
            {
                Id = Guid.NewGuid().ToString(),
                Type = StateMachineDefinitionStateTypes.Inject,
                DataStr = "{}"
            };
        }
    }
}
