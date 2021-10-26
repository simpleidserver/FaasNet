using FaasNet.Runtime.Domains;
using Newtonsoft.Json;

namespace FaasNet.Runtime.Builders
{
    public class InjectStateBuilder : BaseStateBuilder<WorkflowDefinitionInjectState>
    {
        internal InjectStateBuilder() : base(WorkflowDefinitionInjectState.Create())
        {
        }

        public IStateBuilder Data(object data)
        {
            StateDef.DataStr = JsonConvert.SerializeObject(data);
            return this;
        }
    }
}
