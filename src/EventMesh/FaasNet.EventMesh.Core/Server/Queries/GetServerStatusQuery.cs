using FaasNet.EventMesh.Core.Server.Queries.Results;
using MediatR;

namespace FaasNet.EventMesh.Core.Server.Queries
{
    public class GetServerStatusQuery : IRequest<ServerStatusResult>
    {
    }
}
