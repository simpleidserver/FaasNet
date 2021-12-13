using FaasNet.Gateway.Core.Common;
using FaasNet.Gateway.Core.Extensions;
using FaasNet.Gateway.Core.StateMachineInstances.Results;
using FaasNet.Runtime.Persistence;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.StateMachineInstances.Queries.Handlers
{
    public class SearchStateMachineInstanceQueryHandler : IRequestHandler<SearchStateMachineInstanceQuery, BaseSearchResult<StateMachineInstanceResult>>
    {
        private static Dictionary<string, string> MAPPING_STATEMACHINEINSTANCE_TO_PROPERTYNAME = new Dictionary<string, string>
        {
            { "id", "Id" },
            { "workflowDefId", "WorkflowDefId" },
            { "workflowDefName", "WorkflowDefName" },
            { "workflowDefDescription", "WorkflowDefDescription" },
            { "createDateTime", "CreateDateTime" }
        };
        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;

        public SearchStateMachineInstanceQueryHandler(IWorkflowInstanceRepository workflowInstanceRepository)
        {
            _workflowInstanceRepository = workflowInstanceRepository;
        }

        public Task<BaseSearchResult<StateMachineInstanceResult>> Handle(SearchStateMachineInstanceQuery request, CancellationToken cancellationToken)
        {
            var query = _workflowInstanceRepository.Query();
            if (MAPPING_STATEMACHINEINSTANCE_TO_PROPERTYNAME.ContainsKey(request.OrderBy))
            {
                query = query.InvokeOrderBy(MAPPING_STATEMACHINEINSTANCE_TO_PROPERTYNAME[request.OrderBy], request.Order);
            }

            int totalLength = query.Count();
            query = query.Skip(request.StartIndex).Take(request.Count);
            return Task.FromResult(new BaseSearchResult<StateMachineInstanceResult>
            {
                StartIndex = request.StartIndex,
                Count = request.Count,
                TotalLength = totalLength,
                Content = query.Select(r => StateMachineInstanceResult.ToDto(r)).ToList()
            });
        }
    }
}
