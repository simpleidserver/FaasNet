using EventMesh.Runtime.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace EventMesh.Runtime
{
    public class EventMeshRuntimeHostBuilder
    {
        private IServiceCollection _serviceCollection;

        public EventMeshRuntimeHostBuilder()
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddTransient<IEventMeshRuntimeHost, EventMeshRuntimeHost>();
            _serviceCollection.AddTransient<IMessageHandler, HeartbeatMessageHandler>();
        }

        public IEventMeshRuntimeHost Build()
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            return serviceProvider.GetService<IEventMeshRuntimeHost>();
        }
    }
}