using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.EF;
using FaasNet.Gateway.EF.Persistence;
using FaasNet.Runtime;
using Microsoft.EntityFrameworkCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddGatewayEF(this ServerBuilder builder, Action<DbContextOptionsBuilder> optionsBuilder = null)
        {
            var services = builder.Services;
            services.AddDbContext<GatewayDBContext>(optionsBuilder);
            services.AddTransient<IFunctionRepository, FunctionRepository>();
            return builder;
        }
    }
}
