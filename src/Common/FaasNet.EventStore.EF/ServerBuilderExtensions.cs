using FaasNet.EventStore;
using FaasNet.EventStore.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseEF(this ServerBuilder builder, Action<DbContextOptionsBuilder> optionsBuilder = null)
        {
            var services = builder.Services;
            services.AddDbContext<EventStoreDBContext>(optionsBuilder);
            services.AddTransient<IEventStoreSnapshotRepository, EFEventStoreSnapshotRepository>();
            services.AddTransient<ISubscriptionRepository, EFSubscriptionRepository>();
            return builder;
        }
    }
}
