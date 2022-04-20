using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.EventMesh.Runtime.Website
{
    public class RuntimeHostedService : IHostedService
    {
        private readonly IRuntimeHost _runtimeHost;

        public RuntimeHostedService(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            _runtimeHost = scope.ServiceProvider.GetRequiredService<IRuntimeHost>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _runtimeHost.Run(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _runtimeHost.Stop(cancellationToken);
        }

        public async void Restart(CancellationToken cancellationToken)
        {
            await _runtimeHost.Run(cancellationToken);
        }
    }
}
