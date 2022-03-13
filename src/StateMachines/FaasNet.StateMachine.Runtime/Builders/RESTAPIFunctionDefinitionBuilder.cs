using FaasNet.StateMachine.Runtime.Domains.Definitions;

namespace FaasNet.StateMachine.Runtime.Builders
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

        public StateMachineDefinitionFunction Build()
        {
            return new StateMachineDefinitionFunction
            {
                Operation = _operation,
                Name = _name,
                Type = Domains.Enums.StateMachineDefinitionTypes.REST
            };
        }
    }
}
