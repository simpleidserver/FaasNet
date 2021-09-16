using FaasNet.Gateway.Core.ApiDefinitions.Commands.Results;
using FaasNet.Gateway.Core.Domains;
using FaasNet.Gateway.Core.Extensions;
using FaasNet.Gateway.Core.Factories;
using FaasNet.Gateway.Core.Parameters;
using FaasNet.Gateway.Core.Repositories;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.ApiDefinitions.Commands.Handlers
{
    public class InvokeApiDefinitionCommandHandler : IRequestHandler<InvokeApiDefinitionCommand, InvokeApiDefinitionResult>
    {
        private readonly IApiDefinitionRepository _apiDefinitionRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GatewayConfiguration _configuration;

        public InvokeApiDefinitionCommandHandler(
            IApiDefinitionRepository apiDefinitionRepository,
            IHttpClientFactory httpClientFactory,
            IOptions<GatewayConfiguration> configuration)
        {
            _apiDefinitionRepository = apiDefinitionRepository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration.Value;
        }

        public async Task<InvokeApiDefinitionResult> Handle(InvokeApiDefinitionCommand request, CancellationToken cancellationToken)
        {
            var path = request.FullPath.CleanPath();
            var result = await _apiDefinitionRepository.GetByPath(path, cancellationToken);
            if (result == null)
            {
                return InvokeApiDefinitionResult.NoMatch();
            }

            Dictionary<string, string> parameters;
            ApiDefinitionOperation operation;
            if (!result.TryGetOperation(path, out operation, out parameters)) 
            {
                return InvokeApiDefinitionResult.NoMatch();
            }

            var input = request.Content;
            input = await ExecuteWorkflow(operation, operation.GetRootFunction(), request.Content, cancellationToken);
            return InvokeApiDefinitionResult.Match(input);
        }

        private async Task<JObject> ExecuteWorkflow(ApiDefinitionOperation operation, ApiDefinitionFunction func, JObject input, CancellationToken cancellationToken)
        {
            var parameter = new InvokeFunctionParameter
            {
                Name = func.Function,
                Content = new FunctionParameter
                {
                    Configuration = JObject.Parse(func.SerializedConfiguration),
                    Input = input
                }
            };
            using (var httpClient = _httpClientFactory.Build())
            {
                var request = new HttpRequestMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(parameter), Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{_configuration.FunctionApi}/functions/invoke")
                };
                var httpResult = await httpClient.SendAsync(request);
                httpResult.EnsureSuccessStatusCode();
                var str = await httpResult.Content.ReadAsStringAsync();
                input = new JObject();
                if (!string.IsNullOrWhiteSpace(str))
                {
                    input = JObject.Parse(str);
                }
            }
            
            var nextFunc = operation.GetNextFunction(func);
            if (nextFunc == null)
            {
                return input;
            }

            return await ExecuteWorkflow(operation, nextFunc, input, cancellationToken);
        }
    }
}
