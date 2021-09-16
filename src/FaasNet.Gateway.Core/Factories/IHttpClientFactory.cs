using System.Net.Http;

namespace FaasNet.Gateway.Core.Factories
{
    public interface IHttpClientFactory
    {
        HttpClient Build();
    }
}
