using FaasNet.StateMachine.Core;
using FaasNet.StateMachine.Core.Clients;
using FaasNet.StateMachine.Core.Consumers;
using FaasNet.StateMachine.Core.Infrastructure;
using FaasNet.StateMachine.Core.Infrastructure.Handlers;
using FaasNet.StateMachine.Core.Persistence;
using FaasNet.StateMachine.Core.Persistence.InMemory;
using FaasNet.StateMachine.Runtime;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Domains.IntegrationEvents;
using FaasNet.StateMachine.Runtime.Domains.Subscriptions;
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
                var schedulerEdp = new Uri("queue:scheduler");
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<CloudEventConsumer>();
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

            services.AddMediatR(typeof(ErrorCodes));
            var serverBuilder = services.AddRuntime();
            services.RegisterCore().RegisterInMemory();
            return serverBuilder;
        }

        private static IServiceCollection RegisterCore(this IServiceCollection services)
        {
            services.AddTransient<IStateMachineDefLauncher, StateMachineDefLauncher>();
            services.AddTransient<IFunctionService, FunctionService>();
            services.AddTransient<IIntegrationEventProcessor, IntegrationEventProcessor>();
            services.AddTransient<IIntegrationEventHandler<EventListenedEvent>, EventListenedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<EventUnlistenedEvent>, EventUnlistenedEventHandler>();
            return services;
        }

        private static IServiceCollection RegisterInMemory(this IServiceCollection services)
        {
            var defs = new ConcurrentBag<StateMachineDefinitionAggregate>();
            var instances = new ConcurrentBag<StateMachineInstanceAggregate>();
            var cloudEvts = new ConcurrentBag<CloudEventSubscriptionAggregate>();
            services.AddSingleton<IStateMachineDefinitionRepository>(new InMemoryStateMachineDefinitionRepository(defs));
            services.AddSingleton<IStateMachineInstanceRepository>(new InMemoryStateMachineInstanceRepository(instances));
            services.AddSingleton<ICloudEventSubscriptionRepository>(new InMemoryCloudEventSubscriptionRepository(cloudEvts));
            return services;
        }
    }
}
