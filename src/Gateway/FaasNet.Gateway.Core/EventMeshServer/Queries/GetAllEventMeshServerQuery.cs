using FaasNet.Gateway.Core.EventMeshServer.Queries.Results;
using MediatR;
using System.Collections.Generic;

namespace FaasNet.Gateway.Core.EventMeshServer.Queries
{
    public class GetAllEventMeshServerQuery : IRequest<IEnumerable<EventMeshServerResult>>
    {
    }
}
