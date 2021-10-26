using Newtonsoft.Json.Linq;

namespace FaasNet.Function.Parameters
{
    public class FunctionParameter<T>
    {
        public T Configuration { get; set; }
        public JObject Input { get; set; }
    }
}
