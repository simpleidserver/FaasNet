using FaasNet.Application.Core.Repositories;
using FaasNet.Application.EF;
using FaasNet.Application.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Application.Core
{
    public static class AppServerBuilderExtensions
    {
        public static AppServerBuilder UseEF(this AppServerBuilder builder, Action<DbContextOptionsBuilder> optionsBuilder = null)
        {
            var services = builder.Services;
            services.AddDbContext<ApplicationDBContext>(optionsBuilder);
            services.AddTransient<IApplicationDomainQueryRepository, EFApplicationDomainQueryRepository>();
            return builder;
        }
    }
}
