using FaasNet.StateMachineInstance.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseStateMachineInstanceInMemoryStore(this ServerBuilder serverBuilder)
        {
            serverBuilder.Services.AddSingleton<IStateMachineInstanceRepository, InMemoryStateMachineInstanceRepository>();
            return serverBuilder;
        }
    }
}
