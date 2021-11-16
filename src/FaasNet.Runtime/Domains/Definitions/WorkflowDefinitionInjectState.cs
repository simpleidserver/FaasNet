using FaasNet.Runtime.Domains.Enums;
using Newtonsoft.Json.Linq;
using System;

namespace FaasNet.Runtime.Domains.Definitions
{
    public class WorkflowDefinitionInjectState: BaseWorkflowDefinitionFlowableState
    {
        #region Properties

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
