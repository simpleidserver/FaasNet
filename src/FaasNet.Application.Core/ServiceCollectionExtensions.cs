using FaasNet.Application.Core;
using FaasNet.Application.Core.ApplicationDomain.Queries;
using FaasNet.EventStore;
using MediatR;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static AppServerBuilder AddApplication(this IServiceCollection services,
            Action<ApplicationOptions> appOptions = null,
            Action<EventStoreOptions> evtStoreOptions = null,
            Action<ESServerBuilder> evtStoreBuilderCallback = null)
        {
            if (appOptions == null)
            {
                services.Configure<ApplicationOptions>(o => { });
            }
            else
            {
                services.Configure(appOptions);
            }

            var builder = new AppServerBuilder(services);
            services.AddMediatR(typeof(ApplicationOptions));
            services.AddTransient<IQueryProjection, ApplicationDomainQueryProjection>();
            var evtStoreBuilder = services.AddEventStore(evtStoreOptions);
            if (evtStoreBuilderCallback != null)
            {
                evtStoreBuilderCallback(evtStoreBuilder);
            }

            return builder;
        }
    }
}
