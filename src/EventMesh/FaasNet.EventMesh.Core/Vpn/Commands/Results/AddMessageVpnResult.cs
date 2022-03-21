using MediatR;

namespace FaasNet.EventMesh.Core.Vpn.Commands.Results
{
    public class AddMessageVpnResult : IRequest<string>
    {
        public string Id { get; set; }
    }
}
