using FaasNet.Runtime.AsyncAPI.v2.Models.Bindings;
using FaasNet.Runtime.JSchemas;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FaasNet.Runtime.AsyncAPI.v2.Models
{
    public class Components
    {
        /// <summary>
        /// An object to hold reusable Schema Objects.
        /// </summary>
        [JsonProperty("schemas")]
        public IDictionary<string, FaasNetJsonSchema> Schemas { get; set; }

        /// <summary>
        /// An object to hold reusable Message Objects.
        /// </summary>
        [JsonProperty("messages")]
        public IDictionary<string, Message> Messages { get; set; }
        /// <summary>
        /// Security Schemes.
        /// </summary>
        public IDictionary<string, SecurityScheme> SecuritySchemes { get; set; }
        /// <summary>
        /// Channel Binding Objects.
        /// </summary>
        public IDictionary<string, ChannelBindings> ChannelBindings { get; set; }
        /// <summary>
        /// Operation Binding Objects.
        /// </summary>
        public IDictionary<string, OperationBindings> OperationBindings { get; set; }
    }
}
