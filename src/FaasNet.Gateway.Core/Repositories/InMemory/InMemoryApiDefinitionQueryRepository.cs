using FaasNet.Gateway.Core.ApiDefinitions.Queries.Results;
using FaasNet.Gateway.Core.Common;
using FaasNet.Gateway.Core.Domains;
using FaasNet.Gateway.Core.Extensions;
using FaasNet.Gateway.Core.Repositories.Parameters;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Repositories.InMemory
{
    public class InMemoryApiDefinitionQueryRepository : IApiDefinitionQueryRepository
    {
        private static Dictionary<string, string> MAPPING_APIDEF_TO_PROPERTYNAME = new Dictionary<string, string>
        {
            { "name", "Name" },
            { "createDateTime", "CreateDateTime" },
            { "updateDateTime", "UpdateDateTime" }
        };

        private ICollection<ApiDefinitionAggregate> _apiDefinitions;

        public InMemoryApiDefinitionQueryRepository(ICollection<ApiDefinitionAggregate> apiDefinitions)
        {
            _apiDefinitions = apiDefinitions;
        }

        public Task<ApiDefinitionResult> Get(string name, CancellationToken cancellationToken)
        {
            var apiDef = _apiDefinitions.FirstOrDefault(a => a.Name == name);
            if(apiDef == null)
            {
                return Task.FromResult((ApiDefinitionResult)null);
            }

            return Task.FromResult(ApiDefinitionResult.ToDto(apiDef));
        }

        public Task<BaseSearchResult<ApiDefinitionResult>> Search(SearchApisParameter parameter, CancellationToken cancellationToken)
        {
            var result = _apiDefinitions.AsQueryable();
            if (MAPPING_APIDEF_TO_PROPERTYNAME.ContainsKey(parameter.OrderBy))
            {
                result = result.InvokeOrderBy(MAPPING_APIDEF_TO_PROPERTYNAME[parameter.OrderBy], parameter.Order);
            }


            int totalLength = result.Count();
            result = result.Skip(parameter.StartIndex).Take(parameter.Count);
            return Task.FromResult(new BaseSearchResult<ApiDefinitionResult>
            {
                StartIndex = parameter.StartIndex,
                Count = parameter.Count,
                TotalLength = totalLength,
                Content = result.Select(r => ApiDefinitionResult.ToDto(r)).ToList()
            });
        }
    }
}
