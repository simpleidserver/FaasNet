using Newtonsoft.Json.Linq;

namespace FaasNet.Gateway.Core.Parameters
{
    public class FunctionParameter
    {
        public JObject Configuration { get; set; }
        public JObject Input { get; set; }
    }
}
