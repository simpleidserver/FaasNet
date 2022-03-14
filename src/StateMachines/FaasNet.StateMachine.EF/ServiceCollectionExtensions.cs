using FaasNet.StateMachine.Core.Persistence;
using FaasNet.StateMachine.EF;
using FaasNet.StateMachine.EF.Persistence;
using FaasNet.StateMachine.Runtime;
using Microsoft.EntityFrameworkCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder UseEF(this ServerBuilder builder, Action<DbContextOptionsBuilder> optionsBuilder = null)
        {
            var services = builder.Services;
            services.AddDbContext<RuntimeDBContext>(optionsBuilder);
            services.AddTransient<ICloudEventSubscriptionRepository, CloudEventSubscriptionRepository>();
            services.AddTransient<IStateMachineDefinitionRepository, StateMachineDefinitionRepository>();
            services.AddTransient<IStateMachineInstanceRepository, StateMachineInstanceRepository>();
            return builder;
        }
    }
}
