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
            if (callback != null)
            {
                eventMeshRuntime.ServiceCollection.Configure(callback);
            }
            else
            {
                eventMeshRuntime.ServiceCollection.Configure<AMQPOptions>(opt => { });
            }

            eventMeshRuntime.AddInitScript((s) =>
            {
                var amqpOptions = s.GetRequiredService<IOptions<AMQPOptions>>().Value;
                var brokerConfigurationStore = s.GetRequiredService<IBrokerConfigurationStore>();
                var brokerConfiguration = brokerConfigurationStore.Get(amqpOptions.BrokerName);
                if(brokerConfiguration == null)
                {
                    brokerConfiguration = amqpOptions.ToConfiguration();
                    brokerConfigurationStore.Add(brokerConfiguration);
                    brokerConfigurationStore.SaveChanges();
                }
            });
            eventMeshRuntime.ServiceCollection.AddSingleton<IMessageConsumer, AMQPConsumer>();
            return eventMeshRuntime;
        }
    }
}
