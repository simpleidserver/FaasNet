using Newtonsoft.Json;

namespace FaasNet.Function.Core.Helpers
{
    public class PrometheusLabel
    {
        [JsonProperty(PropertyName = "job")]
        public string Job { get; set; }
    }
}
