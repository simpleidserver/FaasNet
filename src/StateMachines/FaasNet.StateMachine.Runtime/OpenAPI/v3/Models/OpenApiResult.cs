using System.Collections.Generic;

namespace FaasNet.StateMachine.Runtime.OpenAPI.v3.Models
{
    public class OpenApiResult
    {
        public string Openapi { get; set; }
        public IEnumerable<OpenApiServerResult> Servers { get; set; }
        public OpenApiInfoResult Info { get; set; }
        public Dictionary<string, Dictionary<string, OpenApiOperationResult>> Paths { get; set; }
        public OpenApiComponentsSchemaResult Components { get; set; }
    }
}
