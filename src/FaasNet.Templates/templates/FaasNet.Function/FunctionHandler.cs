using FaasNet.Function.Parameters;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Function
{
    public class FunctionHandler
    {
        public async Task<JObject> Handle(FunctionParameter<HelloWorldConfiguration> parameter)
        {
            var result = new JObject
            {
                { "content", new JObject
                {
                    { "message", $"Hello '{parameter.Configuration.FirstName}'" }
                }}
            };
            return result;
        }
    }
}
