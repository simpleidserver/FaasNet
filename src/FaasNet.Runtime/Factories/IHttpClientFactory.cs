using System.Net.Http;

namespace FaasNet.Runtime.Factories
{
    public interface IHttpClientFactory
    {
        HttpClient Build();
    }
}
