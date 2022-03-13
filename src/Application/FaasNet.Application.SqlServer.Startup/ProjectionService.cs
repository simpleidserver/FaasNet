using FaasNet.EventStore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Application.SqlServer.Startup
{
    public class ProjectionService : IHostedService
    {
        private readonly IEnumerable<IQueryProjection> _projections;

        public ProjectionService(IServiceScopeFactory serviceScopeFactory)
        {
            var scope = serviceScopeFactory.CreateScope();
            _projections = scope.ServiceProvider.GetRequiredService<IEnumerable<IQueryProjection>>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach(var projection in _projections)
            {
                await projection.Start(cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach(var projection in _projections)
            {
                projection.Stop();
            }

            return Task.CompletedTask;
        }
    }
}
