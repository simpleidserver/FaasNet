using FaasNet.EventMesh.Core.EventMeshServer.Queries.Results;
using MediatR;

namespace FaasNet.EventMesh.Core.EventMeshServer.Commands
{
    public class AddEventMeshServerCommand : IRequest<EventMeshServerResult>
    {
        public bool IsLocalhost { get; set; }
        public string Urn { get; set; }
        public int? Port { get; set; }
    }
}
