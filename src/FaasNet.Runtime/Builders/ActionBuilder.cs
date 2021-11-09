using FaasNet.Runtime.Domains.Definitions;

namespace FaasNet.Runtime.Builders
{
    public class ActionBuilder
    {
        private readonly WorkflowDefinitionAction _workflowDefinitionAction;

        public ActionBuilder(string name)
        {
            _workflowDefinitionAction = new WorkflowDefinitionAction
            {
                Name = name
            };
        }

        public ActionBuilder SetFunctionRef(string referenceName, string argumentsStr)
        {
            _workflowDefinitionAction.FunctionRef = new WorkflowDefinitionFunctionRef
            {
                ReferenceName = referenceName,
                ArgumentsStr = argumentsStr
            };
            return this;
        }

        public ActionBuilder SetActionDataFilter(string fromStateData, string toStateData, string results, bool useResults = true)
        {
            _workflowDefinitionAction.ActionDataFilter = new WorkflowDefinitionActionDataFilter
            {
                FromStateData = fromStateData,
                ToStateData = toStateData,
                Results = results,
                UseResults = useResults
            };
            return this;
        }

        internal WorkflowDefinitionAction Build()
        {
            return _workflowDefinitionAction;
        }
    }
}
