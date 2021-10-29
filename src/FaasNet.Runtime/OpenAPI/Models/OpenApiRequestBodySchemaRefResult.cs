using Newtonsoft.Json;

namespace FaasNet.Runtime.OpenAPI.Models
{
    public class OpenApiRequestBodySchemaRefResult
    {
        [JsonProperty("$ref")]
        public string Ref { get; set; }
    }
}
