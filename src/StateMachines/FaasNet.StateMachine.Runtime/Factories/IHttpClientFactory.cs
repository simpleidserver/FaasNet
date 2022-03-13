using System.Net.Http;

namespace FaasNet.StateMachine.Runtime.Factories
{
    public interface IHttpClientFactory
    {
        HttpClient Build();
    }
}
