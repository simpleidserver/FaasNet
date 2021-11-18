using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Factories;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Queries.Handlers
{
    public class GetFunctionConfigurationQueryHandler : IRequestHandler<GetFunctionConfigurationQuery, JObject>
    {
        private readonly IFunctionQueryRepository _functionRepository;
        private readonly Factories.IHttpClientFactory _httpClientFactory;
        private readonly GatewayConfiguration _configuration;

        public GetFunctionConfigurationQueryHandler(
            IFunctionQueryRepository functionRepository,
            Factories.IHttpClientFactory httpClientFactory,
            IOptions<GatewayConfiguration> configuration)
        {
            _functionRepository = functionRepository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration.Value;
        }

        public async Task<JObject> Handle(GetFunctionConfigurationQuery request, CancellationToken cancellationToken)
        {
            var function = await _functionRepository.Get(request.FuncName, cancellationToken);
            if (function == null)
            {
                throw new FunctionNotFoundException(string.Format(Global.UnknownFunction, request.FuncName));
            }

            using (var httpClient = _httpClientFactory.Build())
            {
                var httpRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{_configuration.FunctionApi}/functions/{request.FuncName}/configuration")
                };
                var httpResult = await httpClient.SendAsync(httpRequest);
                httpResult.EnsureSuccessStatusCode();
                var str = await httpResult.Content.ReadAsStringAsync();
                return JObject.Parse(str);
            }
        }
    }
}
