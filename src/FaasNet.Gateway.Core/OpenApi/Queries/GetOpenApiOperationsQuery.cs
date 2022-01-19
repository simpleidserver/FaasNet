using FaasNet.Runtime.OpenAPI.Models;
using MediatR;
using System.Collections.Generic;

namespace FaasNet.Gateway.Core.OpenApi.Queries
{
    public class GetOpenApiOperationsQuery : IRequest<IEnumerable<OpenApiOperationResult>>
    {
        public string Endpoint { get; set; }
    }
}
