using FaasNet.StateMachine.Runtime.Domains.Definitions;

namespace FaasNet.StateMachine.Runtime.Builders
{
    public class ActionBuilder
    {
        private readonly StateMachineDefinitionAction _workflowDefinitionAction;

        public ActionBuilder(string name)
        {
            _workflowDefinitionAction = new StateMachineDefinitionAction
            {
                Name = name
            };
        }

        public ActionBuilder SetFunctionRef(string referenceName, string argumentsStr)
        {
            _workflowDefinitionAction.FunctionRef = new StateMachineDefinitionFunctionRef
            {
                RefName = referenceName,
                ArgumentsStr = argumentsStr
            };
            return this;
        }

        public ActionBuilder SetActionDataFilter(string fromStateData, string toStateData, string results, bool useResults = true)
        {
            _workflowDefinitionAction.ActionDataFilter = new StateMachineDefinitionActionDataFilter
            {
                FromStateData = fromStateData,
                ToStateData = toStateData,
                Results = results,
                UseResults = useResults
            };
            return this;
        }

        internal StateMachineDefinitionAction Build()
        {
            return _workflowDefinitionAction;
        }
    }
}
