using FaasNet.EventMesh.Runtime;
using FaasNet.EventMesh.Runtime.Handlers;
using FaasNet.EventMesh.Runtime.MessageBroker;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddRuntime(this IServiceCollection services, Action<RuntimeOptions> callback = null)
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
            services.AddSingleton<IUdpClientServerFactory, UdpClientServerFactory>();
            services.AddSingleton<IBrokerConfigurationStore>(new BrokerConfigurationStore());
            services.AddSingleton<IVpnStore>(new VpnStore(new List<Vpn>()));
            return new ServerBuilder(services);
        }

        public static IServiceCollection AddInMemoryMessageBroker(this IServiceCollection services, ConcurrentBag<InMemoryTopic> topics)
        {
            services.AddSingleton<IMessageConsumer>(new InMemoryMessageConsumer(topics));
            services.AddSingleton<IMessagePublisher>(new InMemoryMessagePublisher(topics));
            return services;
        }
    }
}
