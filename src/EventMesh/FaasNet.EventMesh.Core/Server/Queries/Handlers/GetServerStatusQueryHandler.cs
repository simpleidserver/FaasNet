using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Core.Server.Queries.Results;
using MediatR;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Server.Queries.Handlers
{
    public class GetServerStatusQueryHandler : IRequestHandler<GetServerStatusQuery, ServerStatusResult>
    {
        private readonly EventMeshOptions _options;

        public GetServerStatusQueryHandler(IOptions<EventMeshOptions> options)
        {
            _options = options.Value;
        }

        public async Task<ServerStatusResult> Handle(GetServerStatusQuery request, CancellationToken cancellationToken)
        {
            var runningClient = new RuntimeClient(_options.Urn, _options.Port);
            var result = new ServerStatusResult();
            try
            {
                await runningClient.HeartBeat();
                result.IsRunning = true;
            }
            catch
            {
                result.IsRunning = false;
            }

            return result;
        }
    }
}
