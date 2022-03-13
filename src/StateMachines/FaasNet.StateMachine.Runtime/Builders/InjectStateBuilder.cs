using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Newtonsoft.Json;

namespace FaasNet.StateMachine.Runtime.Builders
{
    public class InjectStateBuilder : BaseStateBuilder<StateMachineDefinitionInjectState>
    {
        internal InjectStateBuilder() : base(StateMachineDefinitionInjectState.Create())
        {
        }

        public InjectStateBuilder Data(object data)
        {
            StateDef.DataStr = JsonConvert.SerializeObject(data);
            return this;
        }

        public InjectStateBuilder End()
        {
            StateDef.End = true;
            return this;
        }
    }
}
