using FaasNet.Gateway.Core.OpenApi.Results;
using MediatR;

namespace FaasNet.Gateway.Core.OpenApi.Queries
{
    public class GetOpenApiOperationQuery : IRequest<GetOpenApiOperationResult>
    {
        public string Endpoint { get; set; }
        public string OperationId { get; set; }
    }
}
