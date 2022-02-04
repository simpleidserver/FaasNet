using EventMesh.Runtime.AMQP;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventMesh.Runtime
{
    public static class EventMeshRuntimeHostBuilderExtensions
    {
        public static RuntimeHostBuilder AddAMQP(this RuntimeHostBuilder eventMeshRuntime, Action<AMQPOptions> amqpOptions = null)
        {
            if (amqpOptions != null)
            {
                eventMeshRuntime.ServiceCollection.Configure(amqpOptions);
            }
            else
            {
                eventMeshRuntime.ServiceCollection.Configure<AMQPOptions>(opt => { });
            }

            eventMeshRuntime.ServiceCollection.AddSingleton<IMessageConsumer, AMQPConsumer>();
            return eventMeshRuntime;
        }
    }
}
