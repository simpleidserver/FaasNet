using FaasNet.Runtime.Domains.Definitions;
using Newtonsoft.Json.Linq;

namespace FaasNet.Runtime.Processors.States
{
    public class BaseFlowableStateProcessor
    {
        public StateProcessorResult Ok(JToken jObj, BaseWorkflowDefinitionFlowableState stateDef)
        {
            if(stateDef.End)
            {
                return StateProcessorResult.End(jObj);
            }

            return StateProcessorResult.Next(jObj, stateDef.Transition);
        }
    }
}
