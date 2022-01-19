using FaasNet.Runtime.OpenAPI.Models;

namespace FaasNet.Gateway.Core.OpenApi.Results
{
    public class GetOpenApiOperationResult
    {
        public OpenApiOperationResult OpenApiOperation { get; set; }
        public OpenApiComponentsSchemaResult Components { get; set; }
    }
}
