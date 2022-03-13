using Newtonsoft.Json.Linq;

namespace FaasNet.StateMachine.Runtime.CloudEvent.Models
{
    public class CloudEventMessage
    {
        #region Required Attributes

        /// <summary>
        ///  Identifies the event. 
        ///  Consumers MAY assume that Events with identical source and id are duplicates.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        ///  Identifies the context in which an event happened.
        ///  Often this will include information such as the type of the event source, the organization publishing the event or the process that produced the event. 
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// The version of the CloudEvents specification which the event uses.
        /// </summary>
        public string SpecVersion { get; set; }
        /// <summary>
        /// This attribute contains a value describing the type of event related to the originating occurrence.
        /// Often this attribute is used for routing, observability, policy enforcement, etc. 
        /// </summary>
        public string Type { get; set; }

        #endregion

        public JObject Data { get; set; }

        public string UniqueId
        {
            get
            {
                return $"{Id}.{Source}";
            }
        }
    }
}
