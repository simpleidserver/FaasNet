using FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models.Bindings.Amqp;
using FaasNet.StateMachine.Runtime.JSchemas;
using Newtonsoft.Json;
using NJsonSchema.References;

namespace FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models.Bindings
{
    [JsonConverter(typeof(ReferenceConverter))]
    public class ChannelBindings : JsonReferenceBase<ChannelBindings>, IJsonReference
    {
        [JsonProperty("amqp")]
        public AmqpChannelBinding Amqp { get; set; }

        public IJsonReference ActualObject { get; }

        public object PossibleRoot { get; }
    }
}
