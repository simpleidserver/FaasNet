using FaasNet.EventMesh.Protocols;

namespace FaasNet.EventMesh.Service
{
    public class EventMeshProxyWorker : IHostedService
    {
        private readonly IEnumerable<IProxy> _proxyLst;

        public EventMeshProxyWorker(IEnumerable<IProxy> proxyLst)
        {
            _proxyLst = proxyLst;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var proxy in _proxyLst) await proxy.Start();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var proxy in _proxyLst) proxy.Stop();
            return Task.CompletedTask;
        }
    }
}
