using FaasNet.Common;
using FaasNet.StateMachine.Core;
using FaasNet.StateMachine.Core.Clients;
using FaasNet.StateMachine.Core.Persistence;
using FaasNet.StateMachine.Core.Persistence.InMemory;
using FaasNet.StateMachine.Core.StateMachines;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MediatR;
using System;
using System.Collections.Concurrent;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddStateMachine(this IServiceCollection services, 
            Action<StateMachineOptions> callback = null,
            Action<IServiceCollectionBusConfigurator> configureMassTransit = null)
        {
            if(callback == null)
            {
                services.Configure<StateMachineOptions>(o => { });
            }
            else
            {
                services.Configure(callback);
            }

            if (configureMassTransit == null)
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<StateMachineConsumer>();
                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                });
            }
            else
            {
                services.AddMassTransit(configureMassTransit);
            }

            services.AddMediatR(typeof(ErrorCodes))
                .RegisterCore()
                .RegisterInMemory();
            return new ServerBuilder(services);
        }

        private static IServiceCollection RegisterCore(this IServiceCollection services)
        {
            services.AddTransient<IFunctionService, FunctionService>();
            return services;
        }

        private static IServiceCollection RegisterInMemory(this IServiceCollection services)
        {
            var defs = new ConcurrentBag<StateMachineDefinitionAggregate>();
            var instances = new ConcurrentBag<StateMachineInstanceAggregate>();
            services.AddSingleton<IStateMachineDefinitionRepository>(new InMemoryStateMachineDefinitionRepository(defs));
            return services;
        }
    }
}
