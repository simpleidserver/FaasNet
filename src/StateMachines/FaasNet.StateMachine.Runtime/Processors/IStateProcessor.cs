using FaasNet.StateMachine.Runtime.Domains.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Processors
{
    public interface IStateProcessor
    {
        StateMachineDefinitionStateTypes Type { get; }
        Task<StateProcessorResult> Process(StateMachineInstanceExecutionContext executionContext, CancellationToken cancellationToken);
    }
}
