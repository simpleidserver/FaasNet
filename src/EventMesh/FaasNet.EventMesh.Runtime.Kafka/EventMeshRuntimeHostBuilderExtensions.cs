using FaasNet.EventMesh.Runtime.Kafka;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace FaasNet.EventMesh.Runtime
{
    public static class EventMeshRuntimeHostBuilderExtensions
    {
        public static RuntimeHostBuilder AddKafka(this RuntimeHostBuilder eventMeshRuntime, Action<KafkaOptions> callback = null)
        {
            eventMeshRuntime.ServiceCollection.AddKafka(callback);
            eventMeshRuntime.AddInitScript((s) =>
            {
                s.SeedKafkaOptions();
            });
            return eventMeshRuntime;
        }

        public static void SeedKafkaOptions(this IServiceProvider serviceProvider)
        {
            var amqpOptions = serviceProvider.GetRequiredService<IOptions<KafkaOptions>>().Value;
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
