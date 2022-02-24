using EventMesh.Runtime;
using EventMesh.Runtime.Exceptions;
using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.EventMeshServer.Commands.Handlers
{
    public class AddEventMeshServerBridgeCommandHandler : IRequestHandler<AddEventMeshServerBridgeCommand, bool>
    {
        private readonly IEventMeshServerRepository _eventMeshServerRepository;
        private readonly ILogger<AddEventMeshServerBridgeCommandHandler> _logger;

        public AddEventMeshServerBridgeCommandHandler(IEventMeshServerRepository eventMeshServerRepository, ILogger<AddEventMeshServerBridgeCommandHandler> logger)
        {
            _eventMeshServerRepository = eventMeshServerRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(AddEventMeshServerBridgeCommand request, CancellationToken cancellationToken)
        {
            var fromServer = await _eventMeshServerRepository.Get(request.From.Urn, request.From.Port);
            if(fromServer == null)
            {
                throw new BadRequestException(ErrorCodes.UnknownEventMeshServer, string.Format(Global.UnknownEventMeshServer, request.From.Urn, request.From.Port));
            }

            var toServer = await _eventMeshServerRepository.Get(request.To.Urn, request.To.Port);
            if(toServer == null)
            {
                throw new BadRequestException(ErrorCodes.UnknownEventMeshServer, string.Format(Global.UnknownEventMeshServer, request.To.Urn, request.To.Port));
            }

            fromServer.AddBridge(request.To.Urn, request.To.Port);
            await AddBridge(request.From, request.To, cancellationToken);
            await _eventMeshServerRepository.Update(fromServer, cancellationToken);
            await _eventMeshServerRepository.SaveChanges(cancellationToken);
            return true;
        }
        private async Task AddBridge(EventMeshServer from, EventMeshServer to, CancellationToken cancellationToken)
        {
            try
            {
                var runtimeClient = new RuntimeClient(from.Urn, from.Port);
                await runtimeClient.AddBridge(to.Urn, to.Port, cancellationToken);
            }
            catch (RuntimeClientException ex)
            {
                _logger.LogError(ex.ToString());
                throw new EventMeshSeverUnreachableException(ex.Message);
            }
        }
    }
}
