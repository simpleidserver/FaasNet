using System.Net.Http;

namespace FaasNet.StateMachine.Runtime.Factories
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient Build()
        {
            return new HttpClient();
        }
    }
}
