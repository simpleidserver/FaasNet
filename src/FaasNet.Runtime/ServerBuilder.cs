using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Persistence;
using FaasNet.Runtime.Persistence.InMemory;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace FaasNet.Runtime
{
    public class ServerBuilder
    {
        public ServerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; private set; }

        public ServerBuilder AddWorkflowDefs(ConcurrentBag<WorkflowDefinitionAggregate> workflowDefs)
        {
            Services.AddSingleton<IWorkflowDefinitionRepository>(new InMemoryWorkflowDefinitionRepository(workflowDefs));
            return this;
        }
    }
}
