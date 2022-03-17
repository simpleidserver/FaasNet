using MediatR;

namespace FaasNet.EventMesh.Core.Vpn.Commands
{
    public class DeleteVpnCommand : IRequest<bool>
    {
        public string Vpn { get; set; }
    }
}
