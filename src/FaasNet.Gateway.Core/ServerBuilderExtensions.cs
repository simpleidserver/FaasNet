using FaasNet.Gateway.Core.Domains;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Repositories.InMemory;
using FaasNet.Runtime;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder AddApiDefs(this ServerBuilder serverBuilder, ICollection<ApiDefinitionAggregate> apiDefs)
        {
            var services = serverBuilder.Services;
            services.AddSingleton<IApiDefinitionCommandRepository>(new InMemoryApiDefinitionCommandRepository(apiDefs));
            services.AddSingleton<IApiDefinitionQueryRepository>(new InMemoryApiDefinitionQueryRepository(apiDefs));
            return serverBuilder;
        }
    }
}
