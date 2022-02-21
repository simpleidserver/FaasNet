using FaasNet.Gateway.Core;
using FaasNet.Gateway.Core.Domains;
using FaasNet.Gateway.Core.Functions;
using FaasNet.Gateway.Core.Functions.Invokers;
using FaasNet.Gateway.Core.Functions.Processors;
using FaasNet.Gateway.Core.Helpers;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Repositories.InMemory;
using FaasNet.Runtime;
using FaasNet.Runtime.Processors;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MaxMind.GeoIP2;
using MediatR;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddGateway(this IServiceCollection services, 
            Action<GatewayConfiguration> callback = null, 
            Action<WebServiceClientOptions> callbackWebServiceOptions = null,
            Action<IServiceCollectionBusConfigurator> configureMassTransit = null)
        {
            if (callback == null)
            {
                services.Configure<GatewayConfiguration>((o) => { });
            }
            else
            {
                services.Configure(callback);
            }

            if(callbackWebServiceOptions == null)
            {
                services.Configure<WebServiceClientOptions>((o) => { });
            }
            else
            {
                services.Configure(callbackWebServiceOptions);
            }

            var builder = new ServerBuilder(services);
            services.AddApi()
                .AddInMemoryStore()
                .AddRuntime(configureMassTransit);
            services.AddHttpClient<WebServiceClient>();
            return builder;
        }

        private static IServiceCollection AddApi(this IServiceCollection services)
        {
            services.AddMediatR(typeof(FunctionService));
            services.AddTransient<IFunctionService, FunctionService>();
            services.AddTransient<IPrometheusHelper, PrometheusHelper>();
            services.AddTransient<IFunctionInvoker, KubernetesFunctionInvoker>();
            services.AddTransient<IFunctionProcessor, CustomFunctionProcessor>();
            return services;
        }

        private static IServiceCollection AddInMemoryStore(this IServiceCollection services)
        {
            var fns = new List<FunctionAggregate>();
            var cmdFnRepository = new InMemoryFunctionRepository(fns);
            services.AddSingleton<IFunctionRepository>(cmdFnRepository);
            services.AddSingleton<IEventMeshServerRepository, InMemoryEventMeshServerRepository>();
            return services;
        }
    }
}
