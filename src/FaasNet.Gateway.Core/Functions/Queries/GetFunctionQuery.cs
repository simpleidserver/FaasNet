using FaasNet.Gateway.Core.Functions.Queries.Results;
using MediatR;

namespace FaasNet.Gateway.Core.Functions.Queries
{
    public class GetFunctionQuery : IRequest<FunctionResult>
    {
        public string Id { get; set; }
    }
}
