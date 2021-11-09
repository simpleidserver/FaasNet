using FaasNet.Runtime.Domains.Definitions;
using Newtonsoft.Json;

namespace FaasNet.Runtime.Builders
{
    public class InjectStateBuilder : BaseStateBuilder<WorkflowDefinitionInjectState>
    {
        internal InjectStateBuilder() : base(WorkflowDefinitionInjectState.Create())
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
