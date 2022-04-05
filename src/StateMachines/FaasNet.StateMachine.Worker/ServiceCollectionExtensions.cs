using FaasNet.Common;
using FaasNet.StateMachine.Runtime.IntegrationEvents;
using FaasNet.StateMachine.Worker;
using FaasNet.StateMachine.Worker.Handlers;
using FaasNet.StateMachine.Worker.Persistence;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddStateMachineWorker(this IServiceCollection services)
        {
            services.AddStateMachineRuntimeCore();
            services.AddEventStore();
            services.AddLock();
            services.AddScoped<IEventConsumerStore, EventConsumerStore>();
            services.AddTransient<IStateMachineLauncher, StateMachineLauncher>();
            services.AddTransient<IIntegrationEventProcessor, IntegrationEventProcessor>();
            services.AddTransient<IIntegrationEventHandler<EventListenedEvent>, EventListenedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<EventUnlistenedEvent>, EventUnlistenedEventHandler>();
            services.AddSingleton<ICloudEventSubscriptionRepository, InMemoryCloudEventSubscriptionRepository>();
            services.AddSingleton<IVpnSubscriptionRepository, InMemoryVpnSubscriptionRepository>();
            return new ServerBuilder(services);
        }
    }
}