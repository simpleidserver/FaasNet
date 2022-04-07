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
            services.AddTransient<IMessageHandler, GetAllVpnsMessageHandler>();
            services.AddSingleton<IUdpClientServerFactory, UdpClientServerFactory>();
            services.AddSingleton<IBrokerConfigurationStore>(new BrokerConfigurationStore());
            services.AddSingleton<IVpnStore>(new VpnStore(new List<Vpn>()));
            services.AddSingleton<IClientStore>(new ClientStore(new List<Client>()));
            services.AddSingleton<IApplicationDomainRepository>(new ApplicationDomainRepository(new List<ApplicationDomain>()));
            services.AddSingleton<IMessageDefinitionRepository>(new MessageDefinitionRepository(new List<MessageDefinition>()));
            return new ServerBuilder(services);
        }

        public static IServiceCollection AddInMemoryMessageBroker(this IServiceCollection services)
        {
            var evts = new ConcurrentBag<EventMeshCloudEvent>();
            services.AddSingleton<IMessageConsumer>(new InMemoryMessageConsumer(evts));
            services.AddSingleton<IMessagePublisher>(new InMemoryMessagePublisher(evts));
            return services;
        }
    }
}
