using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models.Bindings.Amqp
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AmqpChannelBindingExchangeType
    {
        [EnumMember(Value = "topic")]
        Topic,
        [EnumMember(Value = "direct")]
        Direct,
        [EnumMember(Value = "fanout")]
        Fanout,
        [EnumMember(Value = "default")]
        Default,
        [EnumMember(Value = "headers")]
        Headers
    }
}
