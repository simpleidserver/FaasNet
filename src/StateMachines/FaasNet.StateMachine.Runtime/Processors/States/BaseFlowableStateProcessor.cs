using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Newtonsoft.Json.Linq;

namespace FaasNet.StateMachine.Runtime.Processors.States
{
    public abstract class BaseFlowableStateProcessor : BaseStateProcessor
    {
        public StateProcessorResult Ok(JToken jObj, BaseStateMachineFlowableState stateDef)
        {
            // State with a "transition" property.
            if(stateDef.End)
            {
                return StateProcessorResult.End(jObj);
            }

            return StateProcessorResult.Next(jObj, stateDef.Transition);
        }
    }
}
