using Newtonsoft.Json.Linq;

namespace FaasNet.Gateway.Core.ApiDefinitions.Commands.Results
{
    public class InvokeApiDefinitionResult
    {
        public bool MatchApi { get; set; }
        public JObject Content { get; set; }

        public static InvokeApiDefinitionResult NoMatch()
        {
            return new InvokeApiDefinitionResult
            {
                MatchApi = false
            };
        }

        public static InvokeApiDefinitionResult Match(JObject jObj)
        {
            return new InvokeApiDefinitionResult
            {
                MatchApi = true,
                Content = jObj
            };
        }
    }
}
