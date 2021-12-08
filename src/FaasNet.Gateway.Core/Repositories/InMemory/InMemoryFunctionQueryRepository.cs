using FaasNet.Gateway.Core.Common;
using FaasNet.Gateway.Core.Domains;
using FaasNet.Gateway.Core.Extensions;
using FaasNet.Gateway.Core.Functions.Queries.Results;
using FaasNet.Gateway.Core.Repositories.Parameters;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Repositories.InMemory
{
    public class InMemoryFunctionQueryRepository : IFunctionQueryRepository
    {
        private readonly ICollection<FunctionAggregate> _functions;
        private static Dictionary<string, string> MAPPING_FUNCTION_TO_PROPERTYNAME = new Dictionary<string, string>
        {
            { "image", "Image" },
            { "name", "Name" },
            { "createDateTime", "CreateDateTime" },
            { "updateDateTime", "UpdateDateTime" }
        };

        public InMemoryFunctionQueryRepository(ICollection<FunctionAggregate> functions)
        {
            _functions = functions;
        }

        public Task<BaseSearchResult<FunctionResult>> Search(SearchFunctionsParameter parameter, CancellationToken cancellationToken)
        {
            var result = _functions.AsQueryable();
            if (MAPPING_FUNCTION_TO_PROPERTYNAME.ContainsKey(parameter.OrderBy))
            {
                result = result.InvokeOrderBy(MAPPING_FUNCTION_TO_PROPERTYNAME[parameter.OrderBy], parameter.Order);
            }

            int totalLength = result.Count();
            result = result.Skip(parameter.StartIndex).Take(parameter.Count);
            return Task.FromResult(new BaseSearchResult<FunctionResult>
            {
                StartIndex = parameter.StartIndex,
                Count = parameter.Count,
                TotalLength = totalLength,
                Content = result.Select(r => FunctionResult.ToDto(r)).ToList()
            });
        }

        public Task<FunctionResult> Get(string name, CancellationToken cancellationToken)
        {
            var result = _functions.FirstOrDefault(f => f.Name == name);
            if (result == null)
            {
                return Task.FromResult((FunctionResult)null);
            }

            return Task.FromResult(FunctionResult.ToDto(result));
        }
    }
}
