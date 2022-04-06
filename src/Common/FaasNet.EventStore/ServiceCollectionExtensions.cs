using FaasNet.Common;
using FaasNet.EventStore;
using FaasNet.EventStore.InMemory;
using FaasNet.EventStore.Models;
using System;
using System.Collections.Concurrent;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddEventStore(this IServiceCollection services, Action<EventStoreOptions> callback = null)
        {
            if (callback == null)
            {
                services.Configure<EventStoreOptions>(o => { });
            }
            else
            {
                services.Configure(callback);
            }

            var evts = new ConcurrentBag<SerializedEvent>();
            var subscriptions = new ConcurrentBag<EventSubscription>();
            services.AddTransient<ICommitAggregateHelper, CommitAggregateHelper>();
            services.AddSingleton<IEventStoreProducer>(new InMemoryEventStoreProducer(evts, subscriptions));
            services.AddSingleton<IEventStoreConsumer>(new InMemoryEventStoreConsumer(evts, subscriptions));
            services.AddSingleton<IEventStoreSnapshotRepository, InMemoryEventStoreSnapshotRepository>();
            services.AddSingleton<ISubscriptionRepository, InMemorySubscriptionRepository>();
            var builder = new ServerBuilder(services);
            return builder;
        }
    }
}
