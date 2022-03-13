using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Persistence;
using FaasNet.StateMachine.Runtime.Persistence.InMemory;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace FaasNet.StateMachine.Runtime
{
    public class ServerBuilder
    {
        public ServerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; private set; }

        public ServerBuilder AddWorkflowDefs(ConcurrentBag<StateMachineDefinitionAggregate> workflowDefs)
        {
            Services.AddSingleton<IStateMachineDefinitionRepository>(new InMemoryStateMachineDefinitionRepository(workflowDefs));
            return this;
        }
    }
}
