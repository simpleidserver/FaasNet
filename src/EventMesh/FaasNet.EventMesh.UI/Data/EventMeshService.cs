using FaasNet.EventMesh.Client;
using FaasNet.Peer.Client;
using Microsoft.Extensions.Options;

namespace FaasNet.EventMesh.UI.Data
{
    public interface IEventMeshService
    {
        Task<bool> Ping(CancellationToken cancellationToken);
    }

    public class EventMeshService : IEventMeshService
    {
        private IPeerClientFactory _peerClientFactory;
        private readonly EventMeshUIOptions _options;

        public EventMeshService(IPeerClientFactory peerClientFactory, IOptions<EventMeshUIOptions> options)
        {
            _peerClientFactory = peerClientFactory;
            _options = options.Value;
        }

        public async Task<bool> Ping(CancellationToken cancellationToken)
        {
            using (var client = _peerClientFactory.Build<EventMeshClient>(_options.EventMeshUrl, _options.EventMeshPort))
            {
                try
                {
                    var result = await client.Ping(_options.RequestTimeoutMS, cancellationToken);
                    return result != null;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
