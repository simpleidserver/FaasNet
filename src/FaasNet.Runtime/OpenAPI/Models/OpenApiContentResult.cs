using System.Collections.Generic;

namespace FaasNet.Runtime.OpenAPI.Models
{
    public class OpenApiContentResult
    {
        public Dictionary<string, OpenApiRequestBodySchemaResult> Content { get; set; }
    }
}
