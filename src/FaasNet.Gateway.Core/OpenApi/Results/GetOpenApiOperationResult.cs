using FaasNet.Runtime.OpenAPI.v3.Models;

namespace FaasNet.Gateway.Core.OpenApi.Results
{
    public class GetOpenApiApiOperationResult
    {
        public OpenApiOperationResult OpenApiOperation { get; set; }
        public OpenApiComponentsSchemaResult Components { get; set; }
    }
}
