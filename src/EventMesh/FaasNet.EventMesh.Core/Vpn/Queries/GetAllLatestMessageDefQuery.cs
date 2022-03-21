using FaasNet.EventMesh.Core.Vpn.Queries.Results;
using MediatR;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Core.Vpn.Queries
{
    public class GetAllLatestMessageDefQuery : IRequest<IEnumerable<MessageDefinitionResult>>
    {
        public string Vpn { get; set; }
        public string ApplicationDomainId { get; set; }
    }
}
