using FaasNet.Domain;
using FaasNet.StateMachine.Core.StateMachines.Results;
using MediatR;

namespace FaasNet.StateMachine.Core.StateMachines.Queries
{
    public class SearchStateMachinesQuery : BaseSearchParameter, IRequest<BaseSearchResult<StateMachineResult>>
    {
        public SearchStateMachinesQuery()
        {
            OrderBy = "createDateTime";
            Order = SortOrders.DESC;
            StartIndex = 0;
            Count = 100;
        }
    }
}
