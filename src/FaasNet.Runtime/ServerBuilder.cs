using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Persistence;
using FaasNet.Runtime.Persistence.InMemory;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace FaasNet.Runtime
{
    public class ServerBuilder
    {

        private readonly IServiceCollection _services;

        public ServerBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public ServerBuilder AddWorkflowDefs(ConcurrentBag<WorkflowDefinitionAggregate> workflowDefs)
        {
            _services.AddSingleton<IWorkflowDefinitionRepository>(new InMemoryWorkflowDefinitionRepository(workflowDefs));
            return this;
        }
    }
}
