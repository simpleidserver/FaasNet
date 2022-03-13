using FaasNet.Gateway.Core.OpenApi.Results;
using MediatR;

namespace FaasNet.Gateway.Core.AsyncApi.Queries
{
    public class GetAsyncApiOperationQuery : IRequest<GetAsyncApiOperationResult>
    {
        public string Endpoint { get; set; }
        public string OperationId { get; set; }
    }
}
