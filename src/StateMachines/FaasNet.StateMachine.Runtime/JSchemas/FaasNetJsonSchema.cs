using Newtonsoft.Json;
using NJsonSchema;

namespace FaasNet.StateMachine.Runtime.JSchemas
{
    [JsonConverter(typeof(JsonSchemaConverter))]
    public class FaasNetJsonSchema : JsonSchema
    {
    }
}
