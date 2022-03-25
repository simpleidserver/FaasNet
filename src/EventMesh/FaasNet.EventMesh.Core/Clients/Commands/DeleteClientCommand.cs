using MediatR;

namespace FaasNet.EventMesh.Core.Clients.Commands
{
    public class DeleteClientCommand : IRequest<bool>
    {
        public string Vpn { get; set; }
        public string ClientId { get; set; }
    }
}
