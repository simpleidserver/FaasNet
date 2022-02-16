using EventMesh.Runtime.Kafka;
using EventMesh.Runtime.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace EventMesh.Runtime
{
    public static class EventMeshRuntimeHostBuilderExtensions
    {
        public static RuntimeHostBuilder AddKafka(this RuntimeHostBuilder eventMeshRuntime, Action<KafkaOptions> callback = null)
        {
            if (callback != null)
            {
                eventMeshRuntime.ServiceCollection.Configure(callback);
            }
            else
            {
                eventMeshRuntime.ServiceCollection.Configure<KafkaOptions>(opt => { });
            }

            eventMeshRuntime.AddInitScript((s) =>
            {
                var amqpOptions = s.GetRequiredService<IOptions<KafkaOptions>>().Value;
                var brokerConfigurationStore = s.GetRequiredService<IBrokerConfigurationStore>();
                var brokerConfiguration = brokerConfigurationStore.Get(amqpOptions.BrokerName);
                if(brokerConfiguration == null)
                {
                    brokerConfiguration = amqpOptions.ToConfiguration();
                    brokerConfigurationStore.Add(brokerConfiguration);
                    brokerConfigurationStore.SaveChanges();
                }
            });
            eventMeshRuntime.ServiceCollection.AddSingleton<IMessageConsumer, KafkaConsumer>();
            return eventMeshRuntime;
        }
    }
}
