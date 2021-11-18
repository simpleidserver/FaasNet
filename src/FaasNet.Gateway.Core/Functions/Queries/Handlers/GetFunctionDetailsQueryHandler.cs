using FaasNet.Gateway.Core.Exceptions;
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
    public class GetFunctionDetailsQueryHandler : IRequestHandler<GetFunctionDetailsQuery, JObject>
    {
        private readonly IFunctionQueryRepository _functionQueryRepository;
        private readonly Factories.IHttpClientFactory _httpClientFactory;
        private readonly GatewayConfiguration _configuration;

        public GetFunctionDetailsQueryHandler(
            IFunctionQueryRepository functionQueryRepository,
            Factories.IHttpClientFactory httpClientFactory,
            IOptions<GatewayConfiguration> configuration)
        {
            _functionQueryRepository = functionQueryRepository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration.Value;
        }

        public async Task<JObject> Handle(GetFunctionDetailsQuery request, CancellationToken cancellationToken)
        {
            var function = await _functionQueryRepository.Get(request.FuncName, cancellationToken);
            if (function == null)
            {
                throw new FunctionNotFoundException(string.Format(Global.UnknownFunction, request.FuncName));
            }

            using (var httpClient = _httpClientFactory.Build())
            {
                var httpRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{_configuration.FunctionApi}/functions/{request.FuncName}/details")
                };
                var httpResult = await httpClient.SendAsync(httpRequest);
                httpResult.EnsureSuccessStatusCode();
                var str = await httpResult.Content.ReadAsStringAsync();
                return JObject.Parse(str);
            }
        }
    }
}
