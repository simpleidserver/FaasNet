using FaasNet.EventMesh.Core.Vpn.Commands.Results;
using MediatR;

namespace FaasNet.EventMesh.Core.Vpn.Commands
{
    public class AddMessageVpnCommand : IRequest<AddMessageVpnResult>
    {
        public string Vpn { get; set; }
        public string ApplicationDomainId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string JsonSchema { get; set; }
    }
}
