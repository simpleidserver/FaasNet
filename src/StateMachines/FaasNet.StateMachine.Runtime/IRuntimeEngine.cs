using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime
{
    public interface IRuntimeEngine
    {
        Task<StateMachineInstanceAggregate> InstanciateAndLaunch(StateMachineDefinitionAggregate workflowDefinitionAggregate, string input, CancellationToken cancellationToken);
        Task<StateMachineInstanceAggregate> InstanciateAndLaunch(StateMachineDefinitionAggregate workflowDefinitionAggregate, string input, Dictionary<string, string> parameters, CancellationToken cancellationToken);
        Task Launch(StateMachineDefinitionAggregate workflowDefinitionAggregate, StateMachineInstanceAggregate workflowInstance, JObject input, CancellationToken cancellationToken);
        Task Launch(StateMachineDefinitionAggregate workflowDefinitionAggregate, StateMachineInstanceAggregate workflowInstance, JObject input, string stateInstanceId, CancellationToken cancellationToken);
    }
}
