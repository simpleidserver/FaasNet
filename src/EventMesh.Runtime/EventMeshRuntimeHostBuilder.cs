using EventMesh.Runtime.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace EventMesh.Runtime
{
    public class EventMeshRuntimeHostBuilder
    {
        public EventMeshRuntimeHostBuilder()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddTransient<IEventMeshRuntimeHost, EventMeshRuntimeHost>();
            ServiceCollection.AddTransient<IMessageHandler, HeartbeatMessageHandler>();
        }

        public IServiceCollection ServiceCollection { get; }

        public IEventMeshRuntimeHost Build()
        {
            var serviceProvider = ServiceCollection.BuildServiceProvider();
            return serviceProvider.GetService<IEventMeshRuntimeHost>();
        }
    }
}