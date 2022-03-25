using FaasNet.EventMesh.Core.ApplicationDomains.Queries.Results;
using MediatR;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Core.ApplicationDomains.Commands
{
    public class UpdateApplicationDomainCommand : IRequest<bool>
    {
        public string ApplicationDomainId { get; set; }
        public IEnumerable<ApplicationResult> Applications { get; set; }
    }
}
