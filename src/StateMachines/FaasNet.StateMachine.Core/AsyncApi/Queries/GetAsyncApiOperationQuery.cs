using FaasNet.StateMachine.Core.OpenApi.Results;
using MediatR;

namespace FaasNet.StateMachine.Core.AsyncApi.Queries
{
    public class GetAsyncApiOperationQuery : IRequest<GetAsyncApiOperationResult>
    {
        public string Endpoint { get; set; }
        public string OperationId { get; set; }
    }
}
