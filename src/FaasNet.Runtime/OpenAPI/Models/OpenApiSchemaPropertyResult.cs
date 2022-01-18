using Newtonsoft.Json;

namespace FaasNet.Runtime.OpenAPI.Models
{
    public class OpenApiSchemaPropertyResult
    {
        public string Type { get; set; }
        public bool? Nullable { get; set; }
        [JsonProperty("$ref")]
        public string Reference { get; set; }
        public OpenApiSchemaPropertyResult Items { get; set; }
    }
}
