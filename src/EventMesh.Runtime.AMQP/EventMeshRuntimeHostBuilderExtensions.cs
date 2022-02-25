using EventMesh.Runtime.AMQP;
using EventMesh.Runtime.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace EventMesh.Runtime
{
    public static class EventMeshRuntimeHostBuilderExtensions
    {
        public static RuntimeHostBuilder AddAMQP(this RuntimeHostBuilder eventMeshRuntime, Action<AMQPOptions> callback = null)
        {
            eventMeshRuntime.ServiceCollection.AddAMQP(callback);
            eventMeshRuntime.AddInitScript((s) =>
            {
                SeedAMQPOptions(s);
            });
            return eventMeshRuntime;
        }

        public static void SeedAMQPOptions(this IServiceProvider serviceProvider)
        {
            var amqpOptions = serviceProvider.GetRequiredService<IOptions<AMQPOptions>>().Value;
            var brokerConfigurationStore = serviceProvider.GetRequiredService<IBrokerConfigurationStore>();
            var brokerConfiguration = brokerConfigurationStore.Get(amqpOptions.BrokerName);
            if (brokerConfiguration == null)
            {
                brokerConfiguration = amqpOptions.ToConfiguration();
                brokerConfigurationStore.Add(brokerConfiguration);
                brokerConfigurationStore.SaveChanges();
            }
        }
    }
}
