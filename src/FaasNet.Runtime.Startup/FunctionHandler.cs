using FaasNet.Runtime.Startup.Parameters;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Startup
{
    public class FunctionHandler
    {
        public Task<JObject> Handle(FunctionParameter<HelloConfiguration> parameter)
        {
            var message = parameter.Configuration.IsBold ? parameter.Input.ToString().ToUpperInvariant() : parameter.Input.ToString();
            return Task.FromResult(new JObject
            {
                { "message", $"Hello {message}" }
            });
        }
    }
}
