using FaasNet.StateMachine.Runtime.Domains.Definitions;
using System;
using System.Linq;

namespace FaasNet.StateMachine.Runtime.Builders
{
    public class StateMachineDefinitionBuilder
    {
        private StateMachineDefinitionAggregate _instance;

        private StateMachineDefinitionBuilder(string id, int version, string name, string description, string vpn)
        {
            _instance = StateMachineDefinitionAggregate.Create(id, version, name, description, vpn);
        }

        public StateMachineDefinitionBuilder AddFunction(Func<FunctionDefinitionBuilder, IFunctionBuilder> callback)
        {
            var builder = new FunctionDefinitionBuilder();
            var fnBuilder = callback(builder);
            _instance.Functions.Add(fnBuilder.Build());
            return this;
        }

        public StateMachineDefinitionBuilder AddConsumedEvent(string name, string source, string type)
        {
            _instance.Events.Add(new StateMachineDefinitionEvent
            {
                Kind = Domains.Enums.StateMachineDefinitionEventKinds.Consumed,
                Name = name,
                Source = source,
                Type = type
            });
            return this;
        }

        public StateMachineDefinitionBuilder StartsWith(Func<StateDefinitionBuilder, IStateBuilder> callback)
        {
            var builder = new StateDefinitionBuilder();
            var stateBuilder = callback(builder);
            var result = stateBuilder.Build();
            _instance.Start = new StateMachineDefinitionStartState
            {
                StateName = result.Id
            };
            _instance.States.Add(result);
            return this;
        }

        public StateMachineDefinitionBuilder Then(Func<StateDefinitionBuilder, IStateBuilder> callback)
        {
            var builder = new StateDefinitionBuilder();
            var stateBuilder = callback(builder);
            _instance.States.Add(stateBuilder.Build());
            if (_instance.States.Count() > 1) 
            {
                var previousElt = _instance.States.ElementAt(_instance.States.Count() - 2);
                var flowableState = previousElt as BaseStateMachineFlowableState;
                if (flowableState != null)
                {
                    flowableState.Transition = _instance.States.Last().Id;
                }
            }

            return this;
        }

        public StateMachineDefinitionAggregate Build()
        {
            return _instance;
        }

        public static StateMachineDefinitionBuilder New(string id, int version, string name, string description, string vpn)
        {
            return new StateMachineDefinitionBuilder(id, version, name, description, vpn);
        }
    }
}
