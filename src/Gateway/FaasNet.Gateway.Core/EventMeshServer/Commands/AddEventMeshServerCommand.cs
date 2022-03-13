using FaasNet.Gateway.Core.EventMeshServer.Queries.Results;
using MediatR;

namespace FaasNet.Gateway.Core.EventMeshServer.Commands
{
    public class AddEventMeshServerCommand : IRequest<EventMeshServerResult>
    {
        public bool IsLocalhost { get; set; }
        public string Urn { get; set; }
        public int? Port { get; set; }
    }
}
