using Newtonsoft.Json;
using System.Collections.Generic;

namespace FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models
{
    public class Server
    {
        /// <summary>
        /// A URL to the target host. This URL supports Server Variables and MAY be relative, to indicate that the host location is relative to the location where the AsyncAPI document is being served.
        /// Variable substitutions will be made when a variable is named in {brackets}.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
        /// <summary>
        /// The protocol this URL supports for connection.
        /// Supported protocol include, but are not limited to : amqp, amqps, http, https, jms, kafka, kafka-secure, anypointmq, mqtt, secure-mqtt, stomp, stomps, ws, wss, mercure. 
        /// </summary>
        [JsonProperty("protocol")]
        public string Protocol { get; set; }
        /// <summary>
        /// The version of the protocol used for connection. For instance: AMQP 0.9.1, HTTP 2.0, Kafka 1.0.0, etc.
        /// </summary>
        [JsonProperty("protocolVersion")]
        public string ProtocolVersion { get; set; }
        /// <summary>
        /// An optional string describing the host designated by the URL.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// A declaration of which security mechanisms can be used with this server.
        /// The list of values includes alternative security requirement objects
        /// that can be used. Only one of the security requirement objects need to
        /// be satisfied to authorize a connection or operation.
        /// </summary>
        [JsonProperty("security")]
        public IList<Dictionary<string, List<string>>> Security { get; set; }
    }
}
