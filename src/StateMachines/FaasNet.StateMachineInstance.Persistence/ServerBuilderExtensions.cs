using FaasNet.StateMachineInstance.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseStateMachineInstanceInMemory(this ServerBuilder serverBuilder)
        {
            serverBuilder.Services.AddSingleton<IStateMachineInstanceRepository, InMemoryStateMachineInstanceRepository>();
            return serverBuilder;
        }
    }
}
