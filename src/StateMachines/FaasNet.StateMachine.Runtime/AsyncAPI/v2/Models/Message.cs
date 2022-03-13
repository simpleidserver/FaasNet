using FaasNet.StateMachine.Runtime.JSchemas;
using Newtonsoft.Json;
using NJsonSchema.References;

namespace FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models
{
    [JsonConverter(typeof(ReferenceConverter))]
    public class Message : JsonReferenceBase<Message>, IJsonReference
    {
        /// <summary>
        /// Schema definition of the application headers. 
        /// </summary>
        [JsonProperty("headers")]
        public FaasNetJsonSchema Headers { get; set; }
        /// <summary>
        /// Definition of the message payload.
        /// </summary>
        [JsonProperty("payload")]
        public FaasNetJsonSchema Payload { get; set; }
        /// <summary>
        /// A string containing the name of the schema format used to define the message payload. 
        /// </summary>
        [JsonProperty("schemaFormat")]
        public string SchemaFormat { get; set; }
        /// <summary>
        /// The content type to use when encoding/decoding a message's payload.
        /// </summary>
        [JsonProperty("contentType")]
        public string ContentType { get; set; }
        /// <summary>
        /// A machine-friendly name for the message.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// A human-friendly title for the message.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// A short summary of what the message is about.
        /// </summary>
        [JsonProperty("summary")]
        public string Summary { get; set; }
        /// <summary>
        /// A verbose explanation of the message. CommonMark syntax can be used for rich text representation.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("reference")]
        public Message Ref
        {
            get { return Reference; }
        }

        public IJsonReference ActualObject { get; }
        public object PossibleRoot { get; }
    }
}
