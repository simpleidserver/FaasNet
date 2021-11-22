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
using MediatR;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddGateway(this IServiceCollection services, Action<GatewayConfiguration> callback = null, Action<IServiceCollectionBusConfigurator> configureMassTransit = null)
        {
            if (callback == null)
            {
                services.Configure<GatewayConfiguration>((o) => { });
            }
            else
            {
                services.Configure(callback);
            }

            var builder = new ServerBuilder(services);
            services.AddApi()
                .AddInMemoryStore()
                .AddRuntime(configureMassTransit);
            return builder;
        }

        private static IServiceCollection AddApi(this IServiceCollection services)
        {
            services.AddMediatR(typeof(FunctionService));
            services.AddTransient<IFunctionService, FunctionService>();
            services.AddTransient<IPrometheusHelper, PrometheusHelper>();
            services.AddTransient<IFunctionInvokerFactory, FunctionInvokerFactory>();
            services.AddTransient<IFunctionInvoker, KubernetesFunctionInvoker>();
            services.AddTransient<IFunctionProcessor, CustomFunctionProcessor>();
            return services;
        }

        private static IServiceCollection AddInMemoryStore(this IServiceCollection services)
        {
            var fns = new List<FunctionAggregate>();
            var cmdFnRepository = new InMemoryFunctionCommandRepository(fns);
            var queryFnRepository = new InMemoryFunctionQueryRepository(fns);
            services.AddSingleton<IFunctionCommandRepository>(cmdFnRepository);
            services.AddSingleton<IFunctionQueryRepository>(queryFnRepository);
            return services;
        }
    }
}
