
using EventMesh.Runtime;
using EventMesh.Runtime.Acl;
using EventMesh.Runtime.Handlers;
using EventMesh.Runtime.MessageBroker;
using EventMesh.Runtime.Stores;
using System;
using System.Collections.Concurrent;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRuntime(this IServiceCollection services, Action<RuntimeOptions> callback = null)
        {
            if (callback != null)
            {
                services.Configure(callback);
            }
            else
            {
                services.Configure<RuntimeOptions>(opt => { });
            }

            services.AddTransient<IRuntimeHost, RuntimeHost>();
            services.AddTransient<IMessageHandler, HeartbeatMessageHandler>();
            services.AddTransient<IMessageHandler, HelloMessageHandler>();
            services.AddTransient<IMessageHandler, SubscribeMessageHandler>();
            services.AddTransient<IMessageHandler, AsyncMessageToClientAckHandler>();
            services.AddTransient<IMessageHandler, AsyncMessageToServerHandler>();
            services.AddTransient<IMessageHandler, AddBridgeMessageHandler>();
            services.AddTransient<IMessageHandler, DisconnectMessageHandler>();
            services.AddTransient<IMessageHandler, PublishMessageRequestHandler>();
            services.AddTransient<IACLService, ACLService>();
            services.AddSingleton<IUdpClientServerFactory, UdpClientServerFactory>();
            services.AddSingleton<IClientStore>(new ClientStore());
            services.AddSingleton<IBridgeServerStore>(new BridgeServerStore());
            services.AddSingleton<IBrokerConfigurationStore>(new BrokerConfigurationStore());
            return services;
        }

        public static IServiceCollection AddInMemoryMessageBroker(this IServiceCollection services, ConcurrentBag<InMemoryTopic> topics)
        {
            services.AddSingleton<IMessageConsumer>(new InMemoryMessageConsumer(topics));
            services.AddSingleton<IMessagePublisher>(new InMemoryMessagePublisher(topics));
            return services;
        }
    }
}
