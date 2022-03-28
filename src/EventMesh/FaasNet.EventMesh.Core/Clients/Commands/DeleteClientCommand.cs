using MediatR;

namespace FaasNet.EventMesh.Core.Clients.Commands
{
    public class DeleteClientCommand : IRequest<bool>
    {
        public string Id { get; set; }
    }
}
