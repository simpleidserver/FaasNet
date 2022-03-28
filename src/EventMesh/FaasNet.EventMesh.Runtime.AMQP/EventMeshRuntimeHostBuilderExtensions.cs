using FaasNet.EventMesh.Runtime.AMQP;
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
        public static RuntimeHostBuilder AddAMQP(this RuntimeHostBuilder eventMeshRuntime, Action<AMQPOptions> callback = null)
        {
            eventMeshRuntime.ServiceCollection.AddAMQP(callback);
            eventMeshRuntime.AddInitScript(async (s) =>
            {
                await SeedAMQPOptions(s);
            });
            return eventMeshRuntime;
        }

        public static async Task SeedAMQPOptions(this IServiceProvider serviceProvider)
        {
            var amqpOptions = serviceProvider.GetRequiredService<IOptions<AMQPOptions>>().Value;
            var brokerConfigurationStore = serviceProvider.GetRequiredService<IBrokerConfigurationStore>();
            var brokerConfiguration = await brokerConfigurationStore.Get(amqpOptions.BrokerName, CancellationToken.None);
            if (brokerConfiguration == null)
            {
                brokerConfiguration = amqpOptions.ToConfiguration();
                brokerConfigurationStore.Add(brokerConfiguration);
                brokerConfigurationStore.SaveChanges(CancellationToken.None);
            }
        }
    }
}
