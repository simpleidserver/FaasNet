using FaasNet.Runtime.Domains.Definitions;

namespace FaasNet.Runtime.Builders
{
    public class ASYNCAPIFunctionDefinitionBuilder : IFunctionBuilder
    {
        private string _name;
        private string _operation;

        internal ASYNCAPIFunctionDefinitionBuilder(string name, string operation)
        {
            _name = name;
            _operation = operation;
        }

        public WorkflowDefinitionFunction Build()
        {
            return new WorkflowDefinitionFunction
            {
                Operation = _operation,
                Name = _name,
                Type = Domains.Enums.WorkflowDefinitionTypes.ASYNCAPI
            };
        }
    }
}
