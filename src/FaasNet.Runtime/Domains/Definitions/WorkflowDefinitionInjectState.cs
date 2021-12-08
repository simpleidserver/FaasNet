using FaasNet.Runtime.Domains.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using YamlDotNet.Serialization;

namespace FaasNet.Runtime.Domains.Definitions
{
    public class WorkflowDefinitionInjectState: BaseWorkflowDefinitionFlowableState
    {
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

        public static WorkflowDefinitionInjectState Create()
        {
            return new WorkflowDefinitionInjectState
            {
                Id = Guid.NewGuid().ToString(),
                Type = WorkflowDefinitionStateTypes.Inject,
                DataStr = "{}"
            };
        }
    }
}
