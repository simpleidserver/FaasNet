using System.Collections.Generic;

namespace FaasNet.Runtime.OpenAPI.v3.Models
{
    public class OpenApiOperationResult
    {
        public IEnumerable<string> Tags { get; set; }
        public string OperationId { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public OpenApiRequestBodyResult RequestBody { get; set; }
        public IEnumerable<OpenApiParameterResult> Parameters { get; set; }
    }
}
