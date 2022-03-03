using FaasNet.EventMesh.Runtime;
using FaasNet.EventMesh.Runtime.Website.Services;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRuntimeWebsite(this IServiceCollection services, Action<RuntimeOptions> callback = null)
        {
            services.AddRuntime(callback);
            services.AddScoped<BreadcrumbState>();
            services.AddScoped<ViewClientState>();
            return services;
        }
    }
}