using Newtonsoft.Json;
using NJsonSchema;

namespace FaasNet.Runtime.JSchemas
{
    [JsonConverter(typeof(JsonSchemaConverter))]
    public class FaasNetJsonSchema : JsonSchema
    {
    }
}
