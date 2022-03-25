using MediatR;

namespace FaasNet.EventMesh.Core.ApplicationDomains.Commands
{
    public class RemoveApplicationDomainCommand : IRequest<bool>
    {
        public string ApplicationDomainId { get; set; }
    }
}
