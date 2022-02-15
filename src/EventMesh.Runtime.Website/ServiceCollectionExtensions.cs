using EventMesh.Runtime;
using EventMesh.Runtime.Website.Services;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRuntimeWebsite(this IServiceCollection services, Action<RuntimeOptions> callback = null)
        {
            if (callback == null)
            {
                services.Configure<RuntimeOptions>(opt => { });
            }
            else
            {
                services.Configure(callback);
            }

            services.AddScoped<BreadcrumbState>();
            services.AddScoped<ViewClientState>();
            return services;
        }
    }
}
