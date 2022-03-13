using MediatR;

namespace FaasNet.Application.Core.ApplicationDomain.Commands
{
    public class RemoveApplicationDomainCommand : IRequest<bool>
    {
        public string Id { get; set; }
    }
}
