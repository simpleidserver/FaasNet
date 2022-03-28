using FaasNet.EventMesh.Runtime.Kafka;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime
{
    public static class EventMeshRuntimeHostBuilderExtensions
    {
        public static RuntimeHostBuilder AddKafka(this RuntimeHostBuilder eventMeshRuntime, Action<KafkaOptions> callback = null)
        {
            eventMeshRuntime.ServiceCollection.AddKafka(callback);
            eventMeshRuntime.AddInitScript(async (s) =>
            {
                await s.SeedKafkaOptions();
            });
            return eventMeshRuntime;
        }

        public static async Task SeedKafkaOptions(this IServiceProvider serviceProvider)
        {
            var amqpOptions = serviceProvider.GetRequiredService<IOptions<KafkaOptions>>().Value;
            var brokerConfigurationStore = serviceProvider.GetRequiredService<IBrokerConfigurationStore>();
            var brokerConfiguration = await brokerConfigurationStore .Get(amqpOptions.BrokerName, CancellationToken.None);
            if (brokerConfiguration == null)
            {
                brokerConfiguration = amqpOptions.ToConfiguration();
                brokerConfigurationStore.Add(brokerConfiguration);
                await brokerConfigurationStore.SaveChanges(CancellationToken.None);
            }
        }
    }
}
