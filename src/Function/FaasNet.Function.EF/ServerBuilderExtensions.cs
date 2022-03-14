using FaasNet.EventMesh.EF;
using FaasNet.Function.Core.Repositories;
using FaasNet.Function.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Function.Core
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseEF(this ServerBuilder builder, Action<DbContextOptionsBuilder> optionsBuilder = null)
        {
            var services = builder.Services;
            services.AddDbContext<FunctionDBContext>(optionsBuilder);
            services.AddTransient<IFunctionRepository, EFFunctionRepository>();
            return builder;
        }
    }
}
