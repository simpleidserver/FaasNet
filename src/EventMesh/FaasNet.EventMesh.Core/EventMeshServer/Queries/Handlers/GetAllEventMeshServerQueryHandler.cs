using FaasNet.EventMesh.Core.EventMeshServer.Queries.Results;
using FaasNet.EventMesh.Core.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.EventMeshServer.Queries.Handlers
{
    public class GetAllEventMeshServerQueryHandler : IRequestHandler<GetAllEventMeshServerQuery, IEnumerable<EventMeshServerResult>>
    {
        private readonly IEventMeshServerRepository _eventMeshServerRepository;

        public GetAllEventMeshServerQueryHandler(IEventMeshServerRepository eventMeshServerRepository)
        {
            _eventMeshServerRepository = eventMeshServerRepository;
        }

        public async Task<IEnumerable<EventMeshServerResult>> Handle(GetAllEventMeshServerQuery request, CancellationToken cancellationToken)
        {
            var result = await _eventMeshServerRepository.GetAll(cancellationToken);
            return result.Select(r => EventMeshServerResult.ToDto(r));
        }
    }
}
