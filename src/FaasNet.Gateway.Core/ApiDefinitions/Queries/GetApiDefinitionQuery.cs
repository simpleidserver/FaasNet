using FaasNet.Gateway.Core.ApiDefinitions.Queries.Results;
using MediatR;

namespace FaasNet.Gateway.Core.ApiDefinitions.Queries
{
    public class GetApiDefinitionQuery : IRequest<ApiDefinitionResult>
    {
        public string Name { get; set; }
    }
}
