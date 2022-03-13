using FaasNet.Domain;
using FaasNet.StateMachine.Core.StateMachineInstances.Results;
using MediatR;

namespace FaasNet.StateMachine.Core.StateMachineInstances.Queries
{
    public class SearchStateMachineInstanceQuery : BaseSearchParameter, IRequest<BaseSearchResult<StateMachineInstanceResult>>
    {
        public SearchStateMachineInstanceQuery()
        {
            OrderBy = "createDateTime";
            Order = SortOrders.DESC;
            StartIndex = 0;
            Count = 100;
        }
    }
}
