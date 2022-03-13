using FaasNet.StateMachine.Runtime.JSchemas;
using System.Collections.Generic;

namespace FaasNet.StateMachine.Runtime.OpenAPI.v3.Models
{
    public class OpenApiComponentsSchemaResult
    {
        public Dictionary<string, FaasNetJsonSchema> Schemas { get; set; }
    }
}
