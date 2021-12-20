using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
using FaasNet.Runtime.Factories;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Queries.Handlers
{
    public class GetFunctionConfigurationQueryHandler : IRequestHandler<GetFunctionConfigurationQuery, JObject>
    {
        private readonly IFunctionRepository _functionRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GatewayConfiguration _configuration;

        public GetFunctionConfigurationQueryHandler(
            IFunctionRepository functionRepository,
            IHttpClientFactory httpClientFactory,
            IOptions<GatewayConfiguration> configuration)
        {
            _functionRepository = functionRepository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration.Value;
        }

        public async Task<JObject> Handle(GetFunctionConfigurationQuery request, CancellationToken cancellationToken)
        {
            var function = _functionRepository.Query().FirstOrDefault(f => f.Id == request.Id);
            if (function == null)
            {
                throw new FunctionNotFoundException(ErrorCodes.UnknownFunction, string.Format(Global.UnknownFunction, request.Id));
            }

            using (var httpClient = _httpClientFactory.Build())
            {
                var httpRequest = new System.Net.Http.HttpRequestMessage
                {
                    Method = System.Net.Http.HttpMethod.Get,
                    RequestUri = new Uri($"{_configuration.FunctionApi}/functions/{function.Id}/configuration")
                };
                var httpResult = await httpClient.SendAsync(httpRequest);
                httpResult.EnsureSuccessStatusCode();
                var str = await httpResult.Content.ReadAsStringAsync();
                return JObject.Parse(str);
            }
        }
    }
}
