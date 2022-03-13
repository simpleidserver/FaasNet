using System.Net.Http;

namespace FaasNet.Function.Core.Factories
{
    public interface IHttpClientFactory
    {
        HttpClient Build();
    }
}
