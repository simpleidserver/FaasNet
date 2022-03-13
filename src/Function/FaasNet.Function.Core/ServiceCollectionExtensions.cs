using FaasNet.Function.Core;
using FaasNet.Function.Core.Domains;
using FaasNet.Function.Core.Factories;
using FaasNet.Function.Core.Functions.Invokers;
using FaasNet.Function.Core.Helpers;
using FaasNet.Function.Core.Repositories;
using MediatR;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddFunction(this IServiceCollection services, Action<FunctionOptions> callback = null)
        {
            if(callback == null)
            {
                services.Configure<FunctionOptions>(o => { });
            }
            else
            {
                services.Configure(callback);
            }

            services.AddMediatR(typeof(PrometheusHelper));
            services.AddTransient<IPrometheusHelper, PrometheusHelper>();
            services.AddTransient<IHttpClientFactory, HttpClientFactory>();
            services.AddTransient<IFunctionInvoker, KubernetesFunctionInvoker>();
            var functions = new List<FunctionAggregate>();
            services.AddSingleton<IFunctionRepository>(new InMemoryFunctionRepository(functions));
            return new ServerBuilder(services);
        }
    }
}
