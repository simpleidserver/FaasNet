using FaasNet.EventMesh.Core.Vpn.Queries.Results;
using MediatR;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Core.Vpn.Commands
{
    public class UpdateApplicationDomainCommand : IRequest<bool>
    {
        public string Vpn { get; set; }
        public string ApplicationDomainId { get; set; }
        public IEnumerable<ApplicationResult> Applications { get; set; }
    }
}
