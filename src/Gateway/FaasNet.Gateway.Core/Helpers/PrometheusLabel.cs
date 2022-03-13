using Newtonsoft.Json;

namespace FaasNet.Gateway.Core.Helpers
{
    public class PrometheusLabel
    {
        [JsonProperty(PropertyName = "job")]
        public string Job { get; set; }
    }
}
