using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace FaasNet.Runtime.AsyncAPI.v2.Models.Bindings.Amqp
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AmqpChannelBindingIs
    {
        [EnumMember(Value = "routingKey")]
        RoutingKey,
        [EnumMember(Value = "queue")]
        Queue,
    }
}
