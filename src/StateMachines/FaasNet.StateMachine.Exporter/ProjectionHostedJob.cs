using FaasNet.EventStore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Exporter
{
    public interface IProjectionHostedJob
    {
        Task Start();
        void Stop();
    }

    public class ProjectionHostedJob : IProjectionHostedJob
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IEnumerable<IQueryProjection> _projections;

        public ProjectionHostedJob(IEnumerable<IQueryProjection> projections)
        {
            _projections = projections;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task Start()
        {
            foreach(var projection in _projections)
            {
                await projection.Start(_cancellationTokenSource.Token);
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
