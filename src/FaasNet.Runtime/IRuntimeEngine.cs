using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Instances;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime
{
    public interface IRuntimeEngine
    {
        Task<WorkflowInstanceAggregate> InstanciateAndLaunch(WorkflowDefinitionAggregate workflowDefinitionAggregate, string input, CancellationToken cancellationToken);
        Task<WorkflowInstanceAggregate> InstanciateAndLaunch(WorkflowDefinitionAggregate workflowDefinitionAggregate, string input, Dictionary<string, string> parameters, CancellationToken cancellationToken);
        Task Launch(WorkflowDefinitionAggregate workflowDefinitionAggregate, WorkflowInstanceAggregate workflowInstance, JObject input, CancellationToken cancellationToken);
        Task Launch(WorkflowDefinitionAggregate workflowDefinitionAggregate, WorkflowInstanceAggregate workflowInstance, JObject input, string stateInstanceId, CancellationToken cancellationToken);
    }
}
