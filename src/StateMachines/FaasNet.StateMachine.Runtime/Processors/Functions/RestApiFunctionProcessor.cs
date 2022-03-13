using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.OpenAPI;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Processors.Functions
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

        public StateMachineDefinitionTypes Type => StateMachineDefinitionTypes.REST;

        public async Task<JToken> Process(JToken input, StateMachineDefinitionFunction function, StateMachineInstanceState instanceState, Dictionary<string, string> parameters, CancellationToken cancellationToken)
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
