using FaasNet.EventMesh.Core;
using FaasNet.EventMesh.Core.Repositories;
using FaasNet.EventMesh.Core.Repositories.InMemory;
using MaxMind.GeoIP2;
using MediatR;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddEventMesh(this IServiceCollection services, Action<WebServiceClientOptions> callbackWebServiceOptions = null)
        {
            if (callbackWebServiceOptions == null)
            {
                services.Configure<WebServiceClientOptions>((o) => { });
            }
            else
            {
                services.Configure(callbackWebServiceOptions);
            }

            var serverBuilder = new ServerBuilder(services);
            services.AddMediatR(typeof(ErrorCodes));
            services.AddSingleton<IEventMeshServerRepository, InMemoryEventMeshServerRepository>();
            services.AddHttpClient<WebServiceClient>();
            return serverBuilder;
        }
    }
}
