using FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models.Bindings;
using FaasNet.StateMachine.Runtime.JSchemas;
using Newtonsoft.Json;

namespace FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models
{
    public class Operation
    {
        /// <summary>
        /// Unique string used to identify the operation. 
        /// The id MUST be unique among all operations described in the API.
        /// he operationId value is case-sensitive. Tools and libraries MAY use the operationId to uniquely identify an operation, therefore, it is RECOMMENDED to follow common programming naming conventions.
        /// </summary>
        [JsonProperty("operationId")]
        public string OperationId { get; set; }
        /// <summary>
        /// A short summary of what the operation is about.
        /// </summary>
        [JsonProperty("summary")]
        public string Summary { get; set; }
        /// <summary>
        /// A verbose explanation of the operation. CommonMark syntax can be used for rich text representation.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// A definition of the message that will be published or received on this channel.
        /// </summary>
        [JsonProperty("message")]
        public Message Message { get; set; }
        /// <summary>
        /// A free-form map where the keys describe the name of the protocol and the values describe protocol-specific definitions for the operation.
        /// </summary>
        [JsonProperty("bindings")]
        public OperationBindings Bindings { get; set; }
    }
}
