using Newtonsoft.Json.Linq;

namespace FaasNet.Runtime.Parameters
{
    public class FunctionParameter<T>
    {
        public T Configuration { get; set; }
        public JObject Input { get; set; }
    }
}
