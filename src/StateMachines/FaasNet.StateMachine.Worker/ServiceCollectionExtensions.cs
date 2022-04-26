using FaasNet.Common;
using FaasNet.StateMachine.Runtime.IntegrationEvents;
using FaasNet.StateMachine.Worker;
using FaasNet.StateMachine.Worker.Handlers;
using FaasNet.StateMachine.Worker.Persistence;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddStateMachineWorker(this IServiceCollection services, Action<StateMachineWorkerOptions> callback = null)
        {
            if (callback == null) services.Configure<StateMachineWorkerOptions>(o => { });
            if (callback != null) services.Configure(callback);
            services.AddLogging();
            services.AddStateMachineRuntimeCore();
            services.AddEventStore();
            services.AddLock();
            services.AddScoped<IEventConsumerStore, EventConsumerStore>();
            services.AddTransient<IStateMachineLauncher, StateMachineLauncher>();
            services.AddTransient<IIntegrationEventProcessor, IntegrationEventProcessor>();
            services.AddTransient<IIntegrationEventHandler<EventListenedEvent>, EventListenedEventHandler>();
            services.AddSingleton<ICloudEventSubscriptionRepository, InMemoryCloudEventSubscriptionRepository>();
            return new ServerBuilder(services);
        }
    }
}