using FaasNet.StateMachine.Core.Persistence;
using FaasNet.StateMachine.Core.Persistence.InMemory;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace FaasNet.StateMachine.Runtime
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder AddWorkflowDefs(this ServerBuilder serverBuilder, ConcurrentBag<StateMachineDefinitionAggregate> workflowDefs)
        {
            serverBuilder.Services.AddSingleton<IStateMachineDefinitionRepository>(new InMemoryStateMachineDefinitionRepository(workflowDefs));
            return serverBuilder;
        }
    }
}
