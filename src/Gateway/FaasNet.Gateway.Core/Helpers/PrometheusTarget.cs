using Newtonsoft.Json;
using System.Collections.Generic;

namespace FaasNet.Gateway.Core.Helpers
{
    public class PrometheusTarget
    {
        public PrometheusTarget()
        {
            Targets = new List<string>();
        }
        
        [JsonProperty(PropertyName = "labels")]
        public PrometheusLabel Labels { get; set; }
        [JsonProperty(PropertyName = "targets")]
        public List<string> Targets { get; set; }
    }
}
