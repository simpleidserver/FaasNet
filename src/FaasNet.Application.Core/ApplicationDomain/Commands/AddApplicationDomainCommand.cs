using FaasNet.Application.Core.ApplicationDomain.Commands.Results;
using MediatR;

namespace FaasNet.Application.Core.ApplicationDomain.Commands
{
    public class AddApplicationDomainCommand: IRequest<AddApplicationDomainResult>
    {
        public string RootTopic { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
