using Newtonsoft.Json;
using System.Collections.Generic;

namespace FaasNet.Runtime.OpenAPI.Models
{
    public class OpenApiSchemaPropertyResult
    {
        public string Type { get; set; }
        public bool? Nullable { get; set; }
        [JsonProperty("$ref")]
        public string Reference { get; set; }
    }
}
