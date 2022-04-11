using FaasNet.Domain;
using FaasNet.StateMachine.Core.StateMachineInstances.Results;
using FaasNet.StateMachineInstance.Persistence;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.StateMachineInstances.Queries.Handlers
{
    public class SearchStateMachineInstanceQueryHandler : IRequestHandler<SearchStateMachineInstanceQuery, BaseSearchResult<StateMachineInstanceResult>>
    {
        private readonly IStateMachineInstanceRepository _workflowInstanceRepository;

        public SearchStateMachineInstanceQueryHandler(IStateMachineInstanceRepository workflowInstanceRepository)
        {
            _workflowInstanceRepository = workflowInstanceRepository;
        }

        public async Task<BaseSearchResult<StateMachineInstanceResult>> Handle(SearchStateMachineInstanceQuery request, CancellationToken cancellationToken)
        {
            var result = await _workflowInstanceRepository.Search(new SearchWorkflowInstanceParameter
            {
                Count = request.Count,
                Order = request.Order,
                OrderBy = request.OrderBy,
                StartIndex = request.StartIndex,
                Vpn = request.Vpn
            }, cancellationToken);
            return new BaseSearchResult<StateMachineInstanceResult>
            {
                StartIndex = request.StartIndex,
                Count = request.Count,
                TotalLength = result.TotalLength,
                Content = result.Content.Select(r => StateMachineInstanceResult.ToDto(r)).ToList()
            };
        }
    }
}
