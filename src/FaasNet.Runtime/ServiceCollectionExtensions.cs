using FaasNet.Runtime;
using FaasNet.Runtime.Consumers;
using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Instances;
using FaasNet.Runtime.Domains.IntegrationEvents;
using FaasNet.Runtime.Domains.Subscriptions;
using FaasNet.Runtime.Factories;
using FaasNet.Runtime.Infrastructure;
using FaasNet.Runtime.Infrastructure.Handlers;
using FaasNet.Runtime.OpenAPI;
using FaasNet.Runtime.Persistence;
using FaasNet.Runtime.Persistence.InMemory;
using FaasNet.Runtime.Processors;
using FaasNet.Runtime.Processors.Functions;
using FaasNet.Runtime.Processors.States;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using System;
using System.Collections.Concurrent;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddRuntime(this IServiceCollection services, Action<IServiceCollectionBusConfigurator> configureMassTransit = null)
        {
            var serverBuilder = new ServerBuilder(services);
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

            services.RegisterInMemory()
                .RegisterCore()
                .AddLogging();
            return serverBuilder;
        }

        private static IServiceCollection RegisterCore(this IServiceCollection services)
        {
            services.AddTransient<IRuntimeEngine, RuntimeEngine>();
            services.AddTransient<IActionExecutor, ActionExecutor>();
            services.AddTransient<IStateProcessor, EventStateProcessor>();
            services.AddTransient<IStateProcessor, InjectStateProcessor>();
            services.AddTransient<IStateProcessor, OperationStateProcessor>();
            services.AddTransient<IStateProcessor, SwitchStateProcessor>();
            services.AddTransient<IFunctionProcessor, RestApiFunctionProcessor>();
            services.AddTransient<IOpenAPIParser, OpenAPIParser>();
            services.AddTransient<IHttpClientFactory, HttpClientFactory>();
            services.AddTransient<IIntegrationEventProcessor, IntegrationEventProcessor>();
            services.AddTransient<IIntegrationEventHandler<EventListenedEvent>, EventListenedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<EventUnlistenedEvent>, EventUnlistenedEventHandler>();
            services.AddSingleton<IDistributedLock, InMemoryDistributedLock>();
            return services;
        }

        private static IServiceCollection RegisterInMemory(this IServiceCollection services)
        {
            var cloudEvts = new ConcurrentBag<CloudEventSubscriptionAggregate>();
            var workflowInstances = new ConcurrentBag<WorkflowInstanceAggregate>();
            var workflowDefs = new ConcurrentBag<WorkflowDefinitionAggregate>();
            services.AddSingleton<ICloudEventSubscriptionRepository>(new InMemoryCloudEventSubscriptionRepository(cloudEvts));
            services.AddSingleton<IWorkflowInstanceRepository>(new InMemoryWorkflowInstanceRepository(workflowInstances));
            services.AddSingleton<IWorkflowDefinitionRepository>(new InMemoryWorkflowDefinitionRepository(workflowDefs));
            return services;
        }
    }
}
