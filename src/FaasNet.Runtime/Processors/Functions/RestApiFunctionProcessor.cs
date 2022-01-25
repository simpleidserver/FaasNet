using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Domains.Instances;
using FaasNet.Runtime.OpenAPI;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors.Functions
{
    public class RestApiFunctionProcessor : IFunctionProcessor
    {
        private readonly IOpenAPIParser _openAPIParser;
        private readonly Factories.IHttpClientFactory _httpClientFactory;

        public RestApiFunctionProcessor(
            IOpenAPIParser openAPIParser,
            Factories.IHttpClientFactory httpClientFactory)
        {
            _openAPIParser = openAPIParser;
            _httpClientFactory = httpClientFactory;
        }

        public WorkflowDefinitionTypes Type => WorkflowDefinitionTypes.REST;

        public async Task<JToken> Process(JToken input, WorkflowDefinitionFunction function, WorkflowInstanceState instanceState, Dictionary<string, string> parameters, CancellationToken cancellationToken)
        {
            OpenAPIUrlResult openApiUrl = null;
            if (!_openAPIParser.TryParseUrl(function.Operation, out openApiUrl))
            {
                var httpClient = _httpClientFactory.Build();
                var httpResult = await httpClient.GetAsync(function.Operation);
                httpResult.EnsureSuccessStatusCode();
                var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
                return JToken.Parse(json);
            }

            return await _openAPIParser.Invoke(openApiUrl.Url, openApiUrl.OperationId, input, cancellationToken);
        }
    }
}
