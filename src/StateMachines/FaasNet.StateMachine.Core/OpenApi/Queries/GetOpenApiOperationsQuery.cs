using FaasNet.StateMachine.Runtime.OpenAPI.v3.Models;
using MediatR;
using System.Collections.Generic;

namespace FaasNet.StateMachine.Core.OpenApi.Queries
{
    public class GetOpenApiOperationsQuery : IRequest<IEnumerable<OpenApiOperationResult>>
    {
        public string Endpoint { get; set; }
    }
}
