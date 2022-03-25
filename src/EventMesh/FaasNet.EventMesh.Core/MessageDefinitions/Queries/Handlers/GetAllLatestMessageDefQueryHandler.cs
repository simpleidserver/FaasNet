using FaasNet.EventMesh.Core.MessageDefinitions.Queries.Results;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.MessageDefinitions.Queries.Handlers
{
    public class GetAllLatestMessageDefQueryHandler : IRequestHandler<GetAllLatestMessageDefQuery, IEnumerable<MessageDefinitionResult>>
    {
        private readonly IMessageDefinitionRepository _messageDefinitionRepository;

        public GetAllLatestMessageDefQueryHandler(IMessageDefinitionRepository messageDefinitionRepository)
        {
            _messageDefinitionRepository = messageDefinitionRepository;
        }

        public async Task<IEnumerable<MessageDefinitionResult>> Handle(GetAllLatestMessageDefQuery request, CancellationToken cancellationToken)
        {
            var messageDefinitions = await _messageDefinitionRepository.GetLatestMessages(request.ApplicationDomainId, cancellationToken);
            return messageDefinitions.Select(s => MessageDefinitionResult.Build(s));
        }
    }
}
