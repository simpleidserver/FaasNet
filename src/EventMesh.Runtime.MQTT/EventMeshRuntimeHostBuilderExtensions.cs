using EventMesh.Runtime.MQTT;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventMesh.Runtime
{
    public static class EventMeshRuntimeHostBuilderExtensions
    {
        public static RuntimeHostBuilder AddMQTT(this RuntimeHostBuilder eventMeshRuntime, Action<MQTTOptions> mqttOptions = null)
        {
            if (mqttOptions != null)
            {
                eventMeshRuntime.ServiceCollection.Configure(mqttOptions);
            }
            else
            {
                eventMeshRuntime.ServiceCollection.Configure<MQTTOptions>(opt => { });
            }

            // eventMeshRuntime.ServiceCollection.AddTransient<IMessageConsumer, MQTTConsumer>();
            return eventMeshRuntime;
        }
    }
}
