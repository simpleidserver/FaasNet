using FaasNet.StateMachine.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.WorkerHost
{
    public class EventConsumerHostedService : IHostedService
    {
        private readonly IEventConsumerStore _eventConsumerStore;

        public EventConsumerHostedService(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            _eventConsumerStore = scope.ServiceProvider.GetRequiredService<IEventConsumerStore>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _eventConsumerStore.Init(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _eventConsumerStore.Stop(cancellationToken);
        }
    }
}
