using FaasNet.Function.Core.Functions.Queries.Results;
using MediatR;

namespace FaasNet.Function.Core.Functions.Queries
{
    public class GetFunctionQuery : IRequest<FunctionResult>
    {
        public string Id { get; set; }
    }
}
