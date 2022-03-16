using MediatR;

namespace FaasNet.EventMesh.Core.Vpn.Commands
{
    public class AddVpnBridgeCommand : IRequest<bool>
    {
        public string Vpn { get; set; }
        public string Urn { get; set; }
        public int Port { get; set; }
    }
}
