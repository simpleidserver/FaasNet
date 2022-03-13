using FaasNet.EventStore.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.EventStore
{
    public static class ESServerBuilderExtensions
    {
        public static ESServerBuilder UseEF(this ESServerBuilder builder, Action<DbContextOptionsBuilder> optionsBuilder = null)
        {
            var services = builder.Services;
            services.AddDbContext<EventStoreDBContext>(optionsBuilder);
            services.AddTransient<IEventStoreSnapshotRepository, EFEventStoreSnapshotRepository>();
            services.AddTransient<ISubscriptionRepository, EFSubscriptionRepository>();
            return builder;
        }
    }
}
