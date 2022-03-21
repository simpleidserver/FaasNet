using MediatR;

namespace FaasNet.EventMesh.Core.Vpn.Commands
{
    public class UpdateMessageVpnCommand : IRequest<bool>
    {
        public string Vpn { get; set; }
        public string ApplicationDomainId { get; set; }
        public string MessageId { get; set; }
        public string Description { get; set; }
        public string JsonSchema { get; set; }
    }
}
