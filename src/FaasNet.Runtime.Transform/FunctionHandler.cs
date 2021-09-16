using FaasNet.Runtime.Parameters;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Transform
{
    public class FunctionHandler
    {
        public Task<JObject> Handle(FunctionParameter<TransformConfiguration> parameter)
        {
            var result = new JObject();
            foreach(var mapping in parameter.Configuration.Mappings)
            {
                var token = parameter.Input.SelectToken(mapping.Input);
                if (token != null)
                {
                    result.Add(mapping.Output, token);
                }
            }
            return Task.FromResult(result);
        }
    }
}
