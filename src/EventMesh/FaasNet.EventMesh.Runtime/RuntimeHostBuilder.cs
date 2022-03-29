using FaasNet.EventMesh.Runtime.MessageBroker;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace FaasNet.EventMesh.Runtime
{
    public class RuntimeHostBuilder
    {
        private ICollection<Action<ServiceProvider>> _initScriptLst;

        public RuntimeHostBuilder(Action<RuntimeOptions> callback = null)
        {
            ServiceCollection = new ServiceCollection();
            _initScriptLst = new List<Action<ServiceProvider>>();
            ServiceCollection.AddRuntime(callback);
        }

        public IServiceCollection ServiceCollection { get; }

        public RuntimeHostBuilder AddVpns(ICollection<Vpn> vpns)
        {
            ServiceCollection.AddSingleton<IVpnStore>(new VpnStore(vpns));
            return this;
        }

        public RuntimeHostBuilder AddClients(ICollection<Models.Client> clients)
        {
            ServiceCollection.AddSingleton<IClientStore>(new ClientStore(clients));
            return this;
        }

        public RuntimeHostBuilder AddInMemoryMessageBroker(ConcurrentBag<InMemoryTopic> topics)
        {
            ServiceCollection.AddInMemoryMessageBroker(topics);
            AddInitScript(async (s) =>
            {
                var brokerConfigurationStore = s.GetRequiredService<IBrokerConfigurationStore>();
                var conf = await brokerConfigurationStore.Get("inmemory", CancellationToken.None);
                if (conf == null)
                {
                    brokerConfigurationStore.Add(new BrokerConfiguration
                    {
                        Name = "inmemory",
                        Protocol = "inmemory"
                    });
                    await brokerConfigurationStore.SaveChanges(CancellationToken.None);
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