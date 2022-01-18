using System.Collections.Generic;

namespace FaasNet.Runtime.OpenAPI.Models
{
    public class OpenApiSchemaResult
    {
        public string Type { get; set; }
        public IEnumerable<string> Required { get; set; }
        public Dictionary<string, OpenApiSchemaPropertyResult> Properties { get; set; }
        public bool AdditionalProperties { get; set; }
    }
}
