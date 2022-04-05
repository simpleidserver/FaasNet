using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Worker
{
    public interface IStateMachineLauncher
    {
        Task<StateMachineInstanceAggregate> InstanciateAndLaunch(StateMachineDefinitionAggregate workflowDefinitionAggregate, string input, CancellationToken cancellationToken);
        Task<StateMachineInstanceAggregate> InstanciateAndLaunch(StateMachineDefinitionAggregate workflowDefinitionAggregate, string input, Dictionary<string, string> parameters, CancellationToken cancellationToken);
    }
}