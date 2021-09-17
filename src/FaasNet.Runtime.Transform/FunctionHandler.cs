using FaasNet.Runtime.Parameters;
using Newtonsoft.Json.Linq;
using System.Linq;
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
                var tokens = parameter.Input.SelectTokens(mapping.Input);
                if (tokens.Count() == 1)
                {
                    result.Add(mapping.Output, tokens.First());
                }
                else if (tokens.Count() > 1)
                {
                    var jArr = new JArray(tokens);
                    result.Add(mapping.Output, jArr);
                }
            }

            return Task.FromResult(result);
        }
    }
}
