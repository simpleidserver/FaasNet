using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace FaasNet.StateMachine.Runtime.OpenAPI.Builders
{
    public interface IRequestBodyBuilder
    {
        string[] ContentTypes { get; }
        HttpContent Build(string contentType, JToken input);
    }
}
