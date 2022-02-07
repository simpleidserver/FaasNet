using EventMesh.Runtime.Acl;
using EventMesh.Runtime.Handlers;
using EventMesh.Runtime.MessageBroker;
using EventMesh.Runtime.Stores;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace EventMesh.Runtime
{
    public class RuntimeHostBuilder
    {
        public RuntimeHostBuilder()
        {
            ServiceCollection = new ServiceCollection();
            var clientStore = new ClientStore();
            ServiceCollection.AddTransient<IRuntimeHost, RuntimeHost>();
            ServiceCollection.AddTransient<IMessageHandler, HeartbeatMessageHandler>();
            ServiceCollection.AddTransient<IMessageHandler, HelloMessageHandler>();
            ServiceCollection.AddTransient<IMessageHandler, SubscribeMessageHandler>();
            ServiceCollection.AddTransient<IMessageHandler, AsyncMessageAckToServerHandler>();
            ServiceCollection.AddTransient<IACLService, ACLService>();
            ServiceCollection.AddSingleton<IClientStore>(clientStore);
        }

        public IServiceCollection ServiceCollection { get; }

        public RuntimeHostBuilder AddInMemoryMessageBroker(ICollection<InMemoryTopic> topics)
        {
            ServiceCollection.AddSingleton<IMessageConsumer>(new InMemoryMessageConsumer(topics));
            ServiceCollection.AddSingleton<IMessagePublisher>(new InMemoryMessagePublisher(topics));
            return this;
        }

        public IRuntimeHost Build()
        {
            var serviceProvider = ServiceCollection.BuildServiceProvider();
            return serviceProvider.GetService<IRuntimeHost>();
        }
    }
}