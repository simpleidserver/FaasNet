using FaasNet.EventStoreDB;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.EventStore
{
    public static class ESServerBuilderExtensions
    {
        public static ESServerBuilder UseEventStoreDB(this ESServerBuilder serverBuilder, Action<EventStoreDBOptions> callback = null)
        {
            var services = serverBuilder.Services;
            if (callback == null)
            {
                services.Configure<EventStoreDBOptions>(o => { });
            }
            else
            {
                services.Configure(callback);
            }

            services.AddTransient<IEventStoreConsumer, EventStoreDBConsumer>();
            services.AddTransient<IEventStoreProducer, EventStoreDBProducer>();
            return serverBuilder;
        }
    }
}
