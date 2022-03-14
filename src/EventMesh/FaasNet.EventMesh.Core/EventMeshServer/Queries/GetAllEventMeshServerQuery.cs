using FaasNet.EventMesh.Core.EventMeshServer.Queries.Results;
using MediatR;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Core.EventMeshServer.Queries
{
    public class GetAllEventMeshServerQuery : IRequest<IEnumerable<EventMeshServerResult>>
    {
    }
}
