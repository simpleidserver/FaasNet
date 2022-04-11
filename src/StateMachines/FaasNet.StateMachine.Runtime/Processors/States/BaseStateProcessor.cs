using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Processors.States
{
    public abstract class BaseStateProcessor : IStateProcessor
    {
        public abstract StateMachineDefinitionStateTypes Type { get; }

        public async Task<StateProcessorResult> Process(StateMachineInstanceExecutionContext executionContext, CancellationToken cancellationToken)
        {
            try
            {
                return await Handle(executionContext, cancellationToken);
            }
            catch(Exception ex)
            {
                return StateProcessorResult.Error(ex);
            }
        }

        protected abstract Task<StateProcessorResult> Handle(StateMachineInstanceExecutionContext executionContext, CancellationToken cancellationToken);

        protected JToken ApplyEventDataFilter(JToken inputStateInstance, StateMachineDefinitionEventDataFilter eventDataFilter, StateMachineInstanceStateEvent stateEvtInstance)
        {
            var output = inputStateInstance;
            if (eventDataFilter != null && eventDataFilter.UseData)
            {
                output.Merge(stateEvtInstance.GetInputDataObj(), eventDataFilter.Data, eventDataFilter.ToStateData);
            }

            return output;
        }
    }
}
