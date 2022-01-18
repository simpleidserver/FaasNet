using System.Collections.Generic;

namespace FaasNet.Runtime.OpenAPI.Models
{
    public class OpenApiResult
    {
        public string Openapi { get; set; }
        public IEnumerable<string> Schemes { get; set; }
        public string BasePath { get; set; }
        public string Host { get; set; }
        public OpenApiInfoResult Info { get; set; }
        public Dictionary<string, Dictionary<string, OpenApiOperationResult>> Paths { get; set; }
        public OpenApiComponentsSchemaResult Components { get; set; }
    }
}
