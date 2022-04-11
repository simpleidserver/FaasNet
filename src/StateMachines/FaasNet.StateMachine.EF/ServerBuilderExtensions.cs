using FaasNet.Common;
using FaasNet.StateMachine.Core.Persistence;
using FaasNet.StateMachine.EF;
using FaasNet.StateMachine.EF.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseStateMachineDefEF(this ServerBuilder builder, Action<DbContextOptionsBuilder> optionsBuilder = null)
        {
            var services = builder.Services;
            services.AddDbContext<RuntimeDBContext>(optionsBuilder);
            services.AddTransient<IStateMachineDefinitionRepository, StateMachineDefinitionRepository>();
            return builder;
        }
    }
}
