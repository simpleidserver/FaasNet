using FaasNet.Runtime.Domains;
using System;

namespace FaasNet.Runtime.Builders
{
    public class RESTAPIFunctionDefinitionBuilder : IFunctionBuilder
    {
        private string _name;
        private string _operation;

        internal RESTAPIFunctionDefinitionBuilder(string name, string operation)
        {
            _name = name;
            _operation = operation;
        }

        public WorkflowDefinitionFunction Build()
        {
            return new WorkflowDefinitionFunction
            {
                Operation = _operation,
                Name = _name
            };
        }
    }
}
