using FaasNet.EventMesh.Core.Vpn.Commands.Results;
using MediatR;

namespace FaasNet.EventMesh.Core.Vpn.Commands
{
    public class AddApplicationDomainCommand: IRequest<AddApplicationDomainResult>
    {
        public string Vpn { get; set; }
        public string RootTopic { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
