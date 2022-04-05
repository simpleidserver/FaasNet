using FaasNet.Common;
using FaasNet.EventStore;
using System;

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

            services.AddTransient<ICommitAggregateHelper, CommitAggregateHelper>();
            var builder = new ServerBuilder(services);
            return builder;
        }
    }
}
