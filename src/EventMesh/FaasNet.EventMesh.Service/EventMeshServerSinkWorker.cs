using FaasNet.EventMesh.Sink;

namespace FaasNet.EventMesh.Service
{
    public class EventMeshServerSinkWorker : IHostedService
    {
        private readonly IEnumerable<ISinkJob> _sinkJobs;

        public EventMeshServerSinkWorker(IEnumerable<ISinkJob> sinkJobs)
        {
            _sinkJobs = sinkJobs;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var job in _sinkJobs) await job.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var job in _sinkJobs) await job.Stop();
        }
    }
}
