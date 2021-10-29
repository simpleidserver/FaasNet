using System.Collections.Generic;

namespace FaasNet.Runtime.OpenAPI.Models
{
    public class OpenApiOperationResult
    {
        public IEnumerable<string> Tags { get; set; }
        public string OperationId { get; set; }
        public OpenApiRequestBodyResult RequestBody { get; set; }
        public IEnumerable<OpenApiParameterResult> Parameters { get; set; }
    }
}
