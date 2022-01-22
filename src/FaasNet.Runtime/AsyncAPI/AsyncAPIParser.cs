using FaasNet.Runtime.AsyncAPI.v2.Converters;
using FaasNet.Runtime.AsyncAPI.v2.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.AsyncAPI
{
    public class AsyncAPIParser : IAsyncAPIParser
    {
        private Factories.IHttpClientFactory _httpClientFactory;

        public AsyncAPIParser(
            Factories.IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task Invoke(string url, string operationId, JToken input, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.Build();
            var httpResult = await httpClient.GetAsync(url, cancellationToken);
            var str = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            var settings = new JsonSerializerSettings
            {
                ReferenceResolverProvider = () => new AsyncApiReferenceResolver()
            };
            var doc = JsonConvert.DeserializeObject<AsyncApiDocument>(str, settings);
        }
    }
}
