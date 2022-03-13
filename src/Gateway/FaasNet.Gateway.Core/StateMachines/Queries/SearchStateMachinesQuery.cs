using FaasNet.Gateway.Core.Common;
using FaasNet.Gateway.Core.Repositories.Parameters;
using FaasNet.Gateway.Core.StateMachines.Results;
using MediatR;

namespace FaasNet.Gateway.Core.StateMachines.Queries
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
