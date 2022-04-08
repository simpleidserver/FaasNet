using FaasNet.StateMachine.Runtime.Domains.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;

namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public class StateMachineDefinitionEvent
    {
        private const string TOPIC_NAME = "topic";

        public StateMachineDefinitionEvent()
        {
            Kind = StateMachineDefinitionEventKinds.Consumed;
        }

        /// <summary>
        /// Unique event name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// CloudEvent source.
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// CloudEvent type.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Defines the event is either consumed or produced by the workflow. 
        /// Default is consumed
        /// </summary>
        public StateMachineDefinitionEventKinds Kind { get; set; }
        /// <summary>
        /// Metadata information.
        /// </summary>
        public JObject Metadata
        {
            get
            {
                return string.IsNullOrWhiteSpace(MetadataStr) ? null : JObject.Parse(MetadataStr);
            }
            set
            {
                MetadataStr = value == null ? null : value.ToString();
            }
        }
        [JsonIgnore]
        [YamlIgnore]
        public string Topic
        {
            get
            {
                if (Metadata == null || !Metadata.ContainsKey(TOPIC_NAME))
                {
                    return null;
                }

                return Metadata[TOPIC_NAME].ToString();
            }
            set
            {
                var obj = new JObject();
                if (Metadata != null)
                {
                    obj = Metadata;
                }

                if(obj.ContainsKey(TOPIC_NAME))
                {
                    obj[TOPIC_NAME] = value;
                }
                else
                {
                    obj.Add(TOPIC_NAME, value);
                }

                Metadata = obj;
            }
        }

        [JsonIgnore]
        [YamlIgnore]
        public string MetadataStr { get; set; }
    }
}
