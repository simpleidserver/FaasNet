﻿using FaasNet.Runtime;
using FaasNet.Runtime.Consumers;
using FaasNet.Runtime.Domains;
using FaasNet.Runtime.Factories;
using FaasNet.Runtime.OpenAPI;
using FaasNet.Runtime.Persistence;
using FaasNet.Runtime.Persistence.InMemory;
using FaasNet.Runtime.Processors;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using System;
using System.Collections.Generic;

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
                .RegisterCore();
            return serverBuilder;
        }

        private static IServiceCollection RegisterCore(this IServiceCollection services)
        {
            services.AddTransient<IRuntimeEngine, RuntimeEngine>();
            services.AddTransient<IActionExecutor, ActionExecutor>();
            services.AddTransient<IStateProcessor, EventStateProcessor>();
            services.AddTransient<IStateProcessor, InjectStateProcessor>();
            services.AddTransient<IStateProcessor, OperationStateProcessor>();
            services.AddTransient<IFunctionProcessor, RestApiFunctionProcessor>();
            services.AddTransient<IOpenAPIParser, OpenAPIParser>();
            services.AddTransient<IHttpClientFactory, HttpClientFactory>();
            return services;
        }

        private static IServiceCollection RegisterInMemory(this IServiceCollection services)
        {
            var cloudEvts = new List<CloudEventSubscriptionAggregate>();
            var workflowInstances = new List<WorkflowInstanceAggregate>();
            var workflowDefs = new List<WorkflowDefinitionAggregate>();
            services.AddSingleton<ICloudEventSubscriptionRepository>(new InMemoryCloudEventSubscriptionRepository(cloudEvts));
            services.AddSingleton<IWorkflowInstanceRepository>(new InMemoryWorkflowInstanceRepository(workflowInstances));
            services.AddSingleton<IWorkflowDefinitionRepository>(new InMemoryWorkflowDefinitionRepository(workflowDefs));
            return services;
        }
    }
}