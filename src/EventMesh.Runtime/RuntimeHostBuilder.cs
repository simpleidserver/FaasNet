using EventMesh.Runtime.Acl;
using EventMesh.Runtime.Handlers;
using EventMesh.Runtime.MessageBroker;
using EventMesh.Runtime.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace EventMesh.Runtime
{
    public class RuntimeHostBuilder
    {
        private ICollection<Action<ServiceProvider>> _initScriptLst;

        public RuntimeHostBuilder(Action<RuntimeOptions> callback = null)
        {
            ServiceCollection = new ServiceCollection();
            _initScriptLst = new List<Action<ServiceProvider>>();
            if (callback != null)
            {
                ServiceCollection.Configure(callback);
            }
            else
            {
                ServiceCollection.Configure<RuntimeOptions>(opt => { });
            }

            ServiceCollection.AddTransient<IRuntimeHost, RuntimeHost>();
            ServiceCollection.AddTransient<IMessageHandler, HeartbeatMessageHandler>();
            ServiceCollection.AddTransient<IMessageHandler, HelloMessageHandler>();
            ServiceCollection.AddTransient<IMessageHandler, SubscribeMessageHandler>();
            ServiceCollection.AddTransient<IMessageHandler, AsyncMessageToClientAckHandler>();
            ServiceCollection.AddTransient<IMessageHandler, AsyncMessageToServerHandler>();
            ServiceCollection.AddTransient<IMessageHandler, AddBridgeMessageHandler>();
            ServiceCollection.AddTransient<IMessageHandler, DisconnectMessageHandler>();
            ServiceCollection.AddTransient<IACLService, ACLService>();
            ServiceCollection.AddSingleton<IUdpClientServerFactory, UdpClientServerFactory>();
            ServiceCollection.AddSingleton<IClientStore>(new ClientStore());
            ServiceCollection.AddSingleton<IBridgeServerStore>(new BridgeServerStore());
        }

        public IServiceCollection ServiceCollection { get; }

        public RuntimeHostBuilder AddInMemoryMessageBroker(ICollection<InMemoryTopic> topics)
        {
            ServiceCollection.AddSingleton<IMessageConsumer>(new InMemoryMessageConsumer(topics));
            ServiceCollection.AddSingleton<IMessagePublisher>(new InMemoryMessagePublisher(topics));
            ServiceCollection.AddSingleton<IBrokerConfigurationStore>(new BrokerConfigurationStore());
            AddInitScript((s) =>
            {
                var brokerConfigurationStore = s.GetRequiredService<IBrokerConfigurationStore>();
                var conf = brokerConfigurationStore.Get("inmemory");
                if (conf == null)
                {
                    brokerConfigurationStore.Add(new Models.BrokerConfiguration
                    {
                        Name = "inmemory",
                        Protocol = "inmemory"
                    });
                    brokerConfigurationStore.SaveChanges();
                }
            });
            return this;
        }

        public RuntimeHostBuilder AddInitScript(Action<ServiceProvider> callback)
        {
            _initScriptLst.Add(callback);
            return this;
        }

        public IRuntimeHost Build()
        {
            var serviceProvider = ServiceCollection.BuildServiceProvider();
            foreach(var callback in _initScriptLst)
            {
                callback(serviceProvider);
            }

            return serviceProvider.GetService<IRuntimeHost>();
        }
    }
}