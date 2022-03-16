using MediatR;

namespace FaasNet.EventMesh.Core.Vpn.Commands
{
    public class RemoveApplicationDomainCommand : IRequest<bool>
    {
        public string Vpn { get; set; }
        public string ApplicationDomainId { get; set; }
    }
}
