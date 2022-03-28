using FaasNet.EventMesh.Core;
using FaasNet.EventMesh.Core.ApplicationDomains;
using FaasNet.EventMesh.Core.Vpn;
using FaasNet.EventMesh.Runtime;
using MaxMind.GeoIP2;
using MediatR;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddEventMesh(this IServiceCollection services,
            Action<WebServiceClientOptions> callbackWebServiceOptions = null,
            Action<EventMeshOptions> options = null)
        {
            if (callbackWebServiceOptions == null)
            {
                services.Configure<WebServiceClientOptions>((o) => { });
            }
            else
            {
                services.Configure(callbackWebServiceOptions);
            }

            if (options == null)
            {
                services.Configure<EventMeshOptions>((o) => { });
            }
            else
            {
                services.Configure(options);
            }

            var serverBuilder = new ServerBuilder(services);
            services.AddMediatR(typeof(EventMeshOptions));
            services.AddHttpClient<WebServiceClient>();
            services.AddTransient<IVpnService, VpnService>();
            services.AddTransient<IApplicationDomainService, ApplicationDomainService>();
            return serverBuilder;
        }
    }
}
