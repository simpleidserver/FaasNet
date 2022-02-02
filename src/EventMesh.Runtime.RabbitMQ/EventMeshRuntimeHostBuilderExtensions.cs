using EventMesh.Runtime.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventMesh.Runtime
{
    public static class EventMeshRuntimeHostBuilderExtensions
    {
        public static EventMeshRuntimeHostBuilder AddRabbitMQ(this EventMeshRuntimeHostBuilder eventMeshRuntime, Action<RabbitMQOptions> rabbitMqOptions = null)
        {
            eventMeshRuntime.ServiceCollection.Configure(rabbitMqOptions);
            eventMeshRuntime.ServiceCollection.AddTransient<IMessageConsumer, RabbitMQConsumer>();
            return eventMeshRuntime;
        }
    }
}
