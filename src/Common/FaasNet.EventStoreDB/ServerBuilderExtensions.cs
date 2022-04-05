using FaasNet.EventStore;
using FaasNet.EventStoreDB;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseEventStoreDB(this ServerBuilder serverBuilder, Action<EventStoreDBOptions> callback = null)
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
