using System.Collections.Generic;

namespace FaasNet.Runtime.OpenAPI.Models
{
    public class OpenApiRequestBodyResult
    {
        public Dictionary<string, OpenApiRequestBodySchemaResult> Content { get; set; }
    }
}
