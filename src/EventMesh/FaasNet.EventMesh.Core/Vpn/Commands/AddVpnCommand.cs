using MediatR;

namespace FaasNet.EventMesh.Core.Vpn.Commands
{
    public class AddVpnCommand : IRequest<bool>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
