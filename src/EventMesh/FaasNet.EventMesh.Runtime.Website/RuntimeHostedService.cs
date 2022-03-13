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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _runtimeHost.Run();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _runtimeHost.Stop();
            return Task.CompletedTask;
        }

        public void Restart()
        {
            _runtimeHost.Run();
        }
    }
}
