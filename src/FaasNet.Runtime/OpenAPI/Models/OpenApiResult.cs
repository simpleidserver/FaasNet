using System.Collections.Generic;

namespace FaasNet.Runtime.OpenAPI.Models
{
    public class OpenApiResult
    {
        public string Openapi { get; set; }
        public OpenApiInfoResult Info { get; set; }
        public Dictionary<string, Dictionary<string, OpenApiOperationResult>> Paths { get; set; }
    }
}
