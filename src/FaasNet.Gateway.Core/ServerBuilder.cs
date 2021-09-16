using FaasNet.Gateway.Core.Domains;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Repositories.InMemory;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace FaasNet.Gateway.Core
{
    public class ServerBuilder
    {
        private readonly IServiceCollection _services;

        internal ServerBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public ServerBuilder AddApiDefs(ICollection<ApiDefinitionAggregate> apiDefs)
        {
            _services.AddSingleton<IApiDefinitionRepository>(new InMemoryApiDefinitionRepository(apiDefs));
            return this;
        }
    }
}
