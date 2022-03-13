using FaasNet.StateMachine.Runtime.Domains.Enums;
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
    }
}
