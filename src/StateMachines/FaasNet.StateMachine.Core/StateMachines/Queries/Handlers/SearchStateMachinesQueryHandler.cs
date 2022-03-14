using FaasNet.Domain;
using FaasNet.Domain.Extensions;
using FaasNet.StateMachine.Core.Persistence;
using FaasNet.StateMachine.Core.StateMachines.Results;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.StateMachines.Queries.Handlers
{
    public class SearchStateMachinesQueryHandler : IRequestHandler<SearchStateMachinesQuery, BaseSearchResult<StateMachineResult>>
    {
        private static Dictionary<string, string> MAPPING_STATEMACHINE_TO_PROPERTYNAME = new Dictionary<string, string>
        {
            { "id", "Id" },
            { "version", "Version" },
            { "name", "Name" },
            { "description", "Description" },
            { "createDateTime", "CreateDateTime" },
            { "updateDateTime", "UpdateDateTime" }
        };
        private readonly IStateMachineDefinitionRepository _stateMachineDefinitionRepository;

        public SearchStateMachinesQueryHandler(IStateMachineDefinitionRepository stateMachineDefinitionRepository)
        {
            _stateMachineDefinitionRepository = stateMachineDefinitionRepository;
        }

        public Task<BaseSearchResult<StateMachineResult>> Handle(SearchStateMachinesQuery request, CancellationToken cancellationToken)
        {
            var query = _stateMachineDefinitionRepository.Query();
            if (MAPPING_STATEMACHINE_TO_PROPERTYNAME.ContainsKey(request.OrderBy))
            {
                query = query.InvokeOrderBy(MAPPING_STATEMACHINE_TO_PROPERTYNAME[request.OrderBy], request.Order);
            }

            query = query.Where(q => q.IsLast);
            int totalLength = query.Count();
            query = query.Skip(request.StartIndex).Take(request.Count);
            return Task.FromResult(new BaseSearchResult<StateMachineResult>
            {
                StartIndex = request.StartIndex,
                Count = request.Count,
                TotalLength = totalLength,
                Content = query.Select(r => StateMachineResult.ToDto(r)).ToList()
            });
        }
    }
}
