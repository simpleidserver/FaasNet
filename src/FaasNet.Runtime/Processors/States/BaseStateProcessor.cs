using FaasNet.Runtime.Domains.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors.States
{
    public abstract class BaseStateProcessor : IStateProcessor
    {
        public abstract WorkflowDefinitionStateTypes Type { get; }

        public async Task<StateProcessorResult> Process(WorkflowInstanceExecutionContext executionContext, CancellationToken cancellationToken)
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

        protected abstract Task<StateProcessorResult> Handle(WorkflowInstanceExecutionContext executionContext, CancellationToken cancellationToken);
    }
}
