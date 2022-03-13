using FaasNet.StateMachine.Runtime.JSchemas;
using NJsonSchema;
using System.Collections.Generic;

namespace FaasNet.StateMachine.Runtime.OpenAPI.v3.Models
{
    public class OpenApiSchemaResult
    {
        public string Type { get; set; }
        public IEnumerable<string> Required { get; set; }
        public Dictionary<string, FaasNetJsonSchema> Properties { get; set; }
        public bool AdditionalProperties { get; set; }
    }
}
