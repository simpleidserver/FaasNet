using FaasNet.Gateway.Core.OpenApi.Results;
using MediatR;

namespace FaasNet.StateMachine.Core.OpenApi.Queries
{
    public class GetOpenApiOperationQuery : IRequest<GetOpenApiApiOperationResult>
    {
        public string Endpoint { get; set; }
        public string OperationId { get; set; }
    }
}
