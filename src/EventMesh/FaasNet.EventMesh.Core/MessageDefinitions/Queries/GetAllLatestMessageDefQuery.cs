using FaasNet.EventMesh.Core.MessageDefinitions.Queries.Results;
using MediatR;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Core.MessageDefinitions.Queries
{
    public class GetAllLatestMessageDefQuery : IRequest<IEnumerable<MessageDefinitionResult>>
    {
        public string ApplicationDomainId { get; set; }
    }
}
