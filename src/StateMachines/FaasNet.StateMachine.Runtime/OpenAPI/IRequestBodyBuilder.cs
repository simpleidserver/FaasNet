using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace FaasNet.StateMachine.Runtime.OpenAPI
{
    public interface IRequestBodyBuilder
    {
        string[] ContentTypes { get; }
        HttpContent Build(string contentType, JToken input);
    }
}
