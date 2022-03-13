using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;

namespace FaasNet.StateMachine.Runtime.OpenAPI.Builders
{
    public class JsonRequestBodyBuilder : IRequestBodyBuilder
    {
        public string[] ContentTypes => new string[] { "application/json", "text/json", "application/*+json" };

        public HttpContent Build(string contentType, JToken input)
        {
            return new StringContent(input.ToString(), Encoding.UTF8, contentType);
        }
    }
}
