using FaasNet.Gateway.Core.Common;
using FaasNet.Gateway.Core.Repositories.Parameters;
using FaasNet.Gateway.Core.StateMachineInstances.Results;
using MediatR;

namespace FaasNet.Gateway.Core.StateMachineInstances.Queries
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
