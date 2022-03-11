using FaasNet.EventStore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ESServerBuilder AddEventStore(this IServiceCollection services, Action<EventStoreOptions> callback = null)
        {
            if (callback == null)
            {
                services.Configure<EventStoreOptions>(o => { });
            }
            else
            {
                services.Configure(callback);
            }

            services.AddTransient<ICommitAggregateHelper, CommitAggregateHelper>();
            var builder = new ESServerBuilder(services);
            return builder;
        }
    }
}
