using FaasNet.Runtime.Domains;
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

        public WorkflowDefinitionBuilder StartsWith(Func<StateDefinitionBuilder, IStateBuilder> callback)
        {
            var builder = new StateDefinitionBuilder();
            var stateBuilder = callback(builder);
            _instance.States.Add(stateBuilder.Build());
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
                previousElt.Transition = _instance.States.Last().Id;
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
