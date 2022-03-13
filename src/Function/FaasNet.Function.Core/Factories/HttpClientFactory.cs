using System.Net.Http;

namespace FaasNet.Function.Core.Factories
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient Build()
        {
            return new HttpClient();
        }
    }
}
