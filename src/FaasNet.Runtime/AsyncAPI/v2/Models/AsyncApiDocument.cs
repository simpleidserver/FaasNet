using Newtonsoft.Json;
using System.Collections.Generic;

namespace FaasNet.Runtime.AsyncAPI.v2.Models
{
    public class AsyncApiDocument
    {
        /// <summary>
        /// Specifies the AsyncAPI Specification version being used.
        /// </summary>
        [JsonProperty("asyncapi")]
        public string AsyncApi { get; }

        /// <summary>
        /// Identifier of the application the AsyncAPI document is defining.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        
        /// <summary>
        /// Provides metadata about the API. The metadata can be used by the clients if needed.
        /// </summary>
        [JsonProperty("info")]
        public Info Info { get; set; }
        
        /// <summary>
        /// Provides connection details of servers.
        /// </summary>
        [JsonProperty("servers")]
        public Dictionary<string, Server> Servers { get; set; }
        // 
        /// <summary>
        /// Default content type to use when encoding/decoding a message's payload.
        /// </summary>
        public string DefaultContentType { get; set; }
        
        /// <summary>
        /// The available channels and messages for the API.
        /// </summary>
        [JsonProperty("channels")]
        public IDictionary<string, ChannelItem> Channels { get; set; }
        
        /// <summary>
        /// An element to hold various schemas for the specification.
        /// </summary>
        [JsonProperty("components")]
        public Components Components { get; set; }
    }
}
