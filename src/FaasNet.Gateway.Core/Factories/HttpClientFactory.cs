using System.Net.Http;

namespace FaasNet.Gateway.Core.Factories
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient Build()
        {
            return new HttpClient();
        }
    }
}
