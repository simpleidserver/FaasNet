using FaasNet.Domain;
using FaasNet.Domain.Extensions;
using FaasNet.Function.Core.Functions.Queries.Results;
using FaasNet.Function.Core.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Function.Core.Functions.Queries.Handlers
{
    public class SearchFunctionsQueryHandler : IRequestHandler<SearchFunctionsQuery, BaseSearchResult<FunctionResult>>
    {
        private static Dictionary<string, string> MAPPING_FUNCTION_TO_PROPERTYNAME = new Dictionary<string, string>
        {
            { "image", "Image" },
            { "name", "Name" },
            { "createDateTime", "CreateDateTime" },
            { "updateDateTime", "UpdateDateTime" }
        };
        private readonly IFunctionRepository _functionRepository;

        public SearchFunctionsQueryHandler(IFunctionRepository functionRepository)
        {
            _functionRepository = functionRepository;
        }

        public Task<BaseSearchResult<FunctionResult>> Handle(SearchFunctionsQuery request, CancellationToken cancellationToken)
        {
            var result = _functionRepository.Query();
            if (MAPPING_FUNCTION_TO_PROPERTYNAME.ContainsKey(request.OrderBy))
            {
                result = result.InvokeOrderBy(MAPPING_FUNCTION_TO_PROPERTYNAME[request.OrderBy], request.Order);
            }

            int totalLength = result.Count();
            result = result.Skip(request.StartIndex).Take(request.Count);
            return Task.FromResult(new BaseSearchResult<FunctionResult>
            {
                StartIndex = request.StartIndex,
                Count = request.Count,
                TotalLength = totalLength,
                Content = result.Select(r => FunctionResult.ToDto(r)).ToList()
            });
        }
    }
}
