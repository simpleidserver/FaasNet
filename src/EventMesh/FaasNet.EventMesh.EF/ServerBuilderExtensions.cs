using FaasNet.EventMesh.Core.Repositories;
using FaasNet.EventMesh.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.EventMesh.Core
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseEF(this ServerBuilder builder, Action<DbContextOptionsBuilder> optionsBuilder = null)
        {
            var services = builder.Services;
            services.AddDbContext<EventMeshDBContext>(optionsBuilder);
            services.AddTransient<IEventMeshServerRepository, EFEventMeshServerRepository>();
            return builder;
        }
    }
}
