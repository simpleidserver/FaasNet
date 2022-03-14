using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Domains;
using FaasNet.EventMesh.Core.EventMeshServer.Queries.Results;
using FaasNet.EventMesh.Core.Repositories;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Runtime;
using FaasNet.EventMesh.Runtime.Exceptions;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.EventMeshServer.Commands.Handlers
{
    public class AddEventMeshServerCommandHandler : IRequestHandler<AddEventMeshServerCommand, EventMeshServerResult>
    {
        private readonly WebServiceClient _maxMindClient;
        private readonly IEventMeshServerRepository _eventMeshServerRepository;
        private readonly ILogger<AddEventMeshServerCommandHandler> _logger;

        public AddEventMeshServerCommandHandler(
            WebServiceClient webServiceClient,
            IEventMeshServerRepository eventMeshServerRepository,
            ILogger<AddEventMeshServerCommandHandler> logger)
        {
            _maxMindClient = webServiceClient;
            _eventMeshServerRepository = eventMeshServerRepository;
            _logger = logger;
        }

        public async Task<EventMeshServerResult> Handle(AddEventMeshServerCommand request, CancellationToken cancellationToken)
        {
            var urn = request.Urn;
            var port = request.Port ?? Constants.DefaultPort;
            var eventMeshServer = await _eventMeshServerRepository.Get(urn, port);
            if (eventMeshServer != null)
            {
                throw new BadRequestException(ErrorCodes.EVENTMESH_SERVER_ALREADY_EXISTS, string.Format(Global.EventMeshServerExists, urn, port));
            }

            await CheckEventMeshServer(urn, port, cancellationToken);
            var city = await GetCity(request.IsLocalhost, request.Urn);
            var record = EventMeshServerAggregate.Create(urn, port, city.Country.IsoCode, city.Postal.Code, city.Location.Latitude, city.Location.Longitude);
            await _eventMeshServerRepository.Add(record, cancellationToken);
            await _eventMeshServerRepository.SaveChanges(cancellationToken);
            return EventMeshServerResult.ToDto(record);
        }

        private async Task CheckEventMeshServer(string urn, int port, CancellationToken cancellationToken)
        {
            try
            {
                var runtimeClient = new RuntimeClient(urn, port);
                await runtimeClient.HeartBeat(cancellationToken);
            }
            catch (RuntimeClientException ex)
            {
                _logger.LogError(ex.ToString());
                throw new BadRequestException(ErrorCodes.UNREACHABLE_EVENTMESH_SERVER, ex.Message);
            }
        }

        private async Task<CityResponse> GetCity(bool isLocalhost, string urn)
        {
            CityResponse cityResponse = null; // new CityResponse(country: new MaxMind.GeoIP2.Model.Country(isoCode: "BE"), postal: new MaxMind.GeoIP2.Model.Postal(code: "1342"), location: new MaxMind.GeoIP2.Model.Location(longitude: 4.555060, latitude: 50.676090));
            if (isLocalhost)
            {
                cityResponse = await _maxMindClient.CityAsync();
            }
            else
            {
                var ipAddress = RuntimeClient.ResolveIPAddress(urn);
                cityResponse = await _maxMindClient.CityAsync(ipAddress);
            }

            return cityResponse;
        }
    }
}
