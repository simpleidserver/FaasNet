﻿using FaasNet.Runtime.Domains;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime
{
    public interface IRuntimeEngine
    {
        Task<WorkflowInstanceAggregate> InstanciateAndLaunch(WorkflowDefinitionAggregate workflowDefinitionAggregate, string input, CancellationToken cancellationToken);
        Task Launch(WorkflowDefinitionAggregate workflowDefinitionAggregate, WorkflowInstanceAggregate workflowInstance, JObject input, CancellationToken cancellationToken);
        Task Launch(WorkflowDefinitionAggregate workflowDefinitionAggregate, WorkflowInstanceAggregate workflowInstance, JObject input, string stateInstanceId, CancellationToken cancellationToken);
    }
}