using FaasNet.Runtime.Domains.Definitions;
using System;
using System.Linq;

namespace FaasNet.Runtime.Builders
{
    public class WorkflowDefinitionBuilder
    {
        private WorkflowDefinitionAggregate _instance;

        private WorkflowDefinitionBuilder(string id, string version, string name, string description)
        {
            _instance = WorkflowDefinitionAggregate.Create(id, version, name, description);
        }

        public WorkflowDefinitionBuilder AddFunction(Func<FunctionDefinitionBuilder, IFunctionBuilder> callback)
        {
            var builder = new FunctionDefinitionBuilder();
            var fnBuilder = callback(builder);
            _instance.Functions.Add(fnBuilder.Build());
            return this;
        }

        public WorkflowDefinitionBuilder AddConsumedEvent(string name, string source, string type)
        {
            _instance.Events.Add(new WorkflowDefinitionEvent
            {
                Kind = Domains.Enums.WorkflowDefinitionEventKinds.Consumed,
                Name = name,
                Source = source,
                Type = type
            });
            return this;
        }

        public WorkflowDefinitionBuilder StartsWith(Func<StateDefinitionBuilder, IStateBuilder> callback)
        {
            var builder = new StateDefinitionBuilder();
            var stateBuilder = callback(builder);
            var result = stateBuilder.Build();
            result.IsRootState = true;
            _instance.States.Add(result);
            return this;
        }

        public WorkflowDefinitionBuilder Then(Func<StateDefinitionBuilder, IStateBuilder> callback)
        {
            var builder = new StateDefinitionBuilder();
            var stateBuilder = callback(builder);
            _instance.States.Add(stateBuilder.Build());
            if (_instance.States.Count() > 1) 
            {
                var previousElt = _instance.States.ElementAt(_instance.States.Count() - 2);
                var flowableState = previousElt as BaseWorkflowDefinitionFlowableState;
                if (flowableState != null)
                {
                    flowableState.Transition = _instance.States.Last().Id;
                }
            }

            return this;
        }

        public WorkflowDefinitionAggregate Build()
        {
            return _instance;
        }

        public static WorkflowDefinitionBuilder New(string id, string version, string name, string description)
        {
            return new WorkflowDefinitionBuilder(id, version, name, description);
        }
    }
}
