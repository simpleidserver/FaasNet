using FaasNet.StateMachine.Runtime;
using FaasNet.StateMachine.Runtime.AsyncAPI;
using FaasNet.StateMachine.Runtime.AsyncAPI.Channels;
using FaasNet.StateMachine.Runtime.AsyncAPI.Channels.Amqp;
using FaasNet.StateMachine.Runtime.Consumers;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Domains.IntegrationEvents;
using FaasNet.StateMachine.Runtime.Domains.Subscriptions;
using FaasNet.StateMachine.Runtime.Factories;
using FaasNet.StateMachine.Runtime.Infrastructure;
using FaasNet.StateMachine.Runtime.Infrastructure.Handlers;
using FaasNet.StateMachine.Runtime.OpenAPI;
using FaasNet.StateMachine.Runtime.OpenAPI.Builders;
using FaasNet.StateMachine.Runtime.Persistence;
using FaasNet.StateMachine.Runtime.Persistence.InMemory;
using FaasNet.StateMachine.Runtime.Processors;
using FaasNet.StateMachine.Runtime.Processors.Functions;
using FaasNet.StateMachine.Runtime.Processors.States;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using System;
using System.Collections.Concurrent;
using v3 = FaasNet.StateMachine.Runtime.OpenAPI.v3;

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
            services.AddTransient<IStateProcessor, ForeachStateProcessor>();
            services.AddTransient<IStateProcessor, CallbackEventStateProcessor>();
            services.AddTransient<IFunctionProcessor, RestApiFunctionProcessor>();
            services.AddTransient<IFunctionProcessor, AsyncApiFunctionProcessor>();
            services.AddTransient<IOpenAPIParser, OpenAPIParser>();
            services.AddTransient<IHttpClientFactory, HttpClientFactory>();
            services.AddTransient<IChannel, AmqpChannel>();
            services.AddTransient<IAmqpChannelClientFactory, AmqpChannelUserPasswordClientFactory>();
            services.AddTransient<IIntegrationEventProcessor, IntegrationEventProcessor>();
            services.AddTransient<IOpenAPIConfigurationParser, v3.OpenAPIConfigurationParser>();
            services.AddTransient<IIntegrationEventHandler<EventListenedEvent>, EventListenedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<EventUnlistenedEvent>, EventUnlistenedEventHandler>();
            services.AddTransient<IRequestBodyBuilder, JsonRequestBodyBuilder>();
            services.AddTransient<IAsyncAPIParser, AsyncAPIParser>();
            services.AddSingleton<IDistributedLock, InMemoryDistributedLock>();
            return services;
        }

        private static IServiceCollection RegisterInMemory(this IServiceCollection services)
        {
            var cloudEvts = new ConcurrentBag<CloudEventSubscriptionAggregate>();
            var workflowInstances = new ConcurrentBag<StateMachineInstanceAggregate>();
            var workflowDefs = new ConcurrentBag<StateMachineDefinitionAggregate>();
            services.AddSingleton<ICloudEventSubscriptionRepository>(new InMemoryCloudEventSubscriptionRepository(cloudEvts));
            services.AddSingleton<IStateMachineInstanceRepository>(new InMemoryStateMachineInstanceRepository(workflowInstances));
            services.AddSingleton<IStateMachineDefinitionRepository>(new InMemoryStateMachineDefinitionRepository(workflowDefs));
            return services;
        }
    }
}
