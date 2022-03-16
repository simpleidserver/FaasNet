using FaasNet.EventMesh.Runtime.MessageBroker;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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

        public RuntimeHostBuilder AddInMemoryMessageBroker(ConcurrentBag<InMemoryTopic> topics)
        {
            ServiceCollection.AddInMemoryMessageBroker(topics);
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