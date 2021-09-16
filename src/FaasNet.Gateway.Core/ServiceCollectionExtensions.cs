using FaasNet.Gateway.Core;
using FaasNet.Gateway.Core.Domains;
using FaasNet.Gateway.Core.Factories;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Repositories.InMemory;
using MediatR;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddGateway(this IServiceCollection services, Action<GatewayConfiguration> callback = null)
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
            var apis = new List<ApiDefinitionAggregate>();
            var apiRepository = new InMemoryApiDefinitionRepository(apis);
            services.AddSingleton<IApiDefinitionRepository>(apiRepository);
            services.AddTransient<IHttpClientFactory, HttpClientFactory>();
            services.AddMediatR(typeof(ServerBuilder));
            return builder;
        }
    }
}
