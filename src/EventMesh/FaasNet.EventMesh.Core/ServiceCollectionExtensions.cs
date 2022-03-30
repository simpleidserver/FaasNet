using FaasNet.EventMesh.Core;
using FaasNet.EventMesh.Core.ApplicationDomains;
using FaasNet.EventMesh.Core.Consumers;
using FaasNet.EventMesh.Core.Vpn;
using FaasNet.EventMesh.Runtime;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MaxMind.GeoIP2;
using MediatR;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddEventMesh(this IServiceCollection services,
            Action<WebServiceClientOptions> callbackWebServiceOptions = null,
            Action<EventMeshOptions> options = null,
            Action<IServiceCollectionBusConfigurator> configureMassTransit = null)
        {
            if (callbackWebServiceOptions == null)
            {
                services.Configure<WebServiceClientOptions>((o) => { });
            }
            else
            {
                services.Configure(callbackWebServiceOptions);
            }

            if (configureMassTransit == null)
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<ApplicationDomainConsumer>();
                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                });
            }
            else
            {
                services.AddMassTransit(x =>
                {
                    configureMassTransit(x);
                });
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
