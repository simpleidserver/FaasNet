using System.Collections.Generic;

namespace FaasNet.Runtime.OpenAPI.v3.Models
{
    public class OpenApiRequestBodyResult
    {
        public Dictionary<string, OpenApiRequestBodySchemaResult> Content { get; set; }
    }
}
