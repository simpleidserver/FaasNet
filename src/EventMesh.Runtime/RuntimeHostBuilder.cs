using EventMesh.Runtime.Acl;
using EventMesh.Runtime.Handlers;
using EventMesh.Runtime.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace EventMesh.Runtime
{
    public class RuntimeHostBuilder
    {
        public RuntimeHostBuilder()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddTransient<IRuntimeHost, RuntimeHost>();
            ServiceCollection.AddTransient<IMessageHandler, HeartbeatMessageHandler>();
            ServiceCollection.AddTransient<IMessageHandler, HelloMessageHandler>();
            ServiceCollection.AddTransient<IMessageHandler, SubscribeMessageHandler>();
            ServiceCollection.AddTransient<IACLService, ACLService>();
            ServiceCollection.AddSingleton<IClientStore, ClientStore>();
        }

        public IServiceCollection ServiceCollection { get; }

        public IRuntimeHost Build()
        {
            var serviceProvider = ServiceCollection.BuildServiceProvider();
            return serviceProvider.GetService<IRuntimeHost>();
        }
    }
}