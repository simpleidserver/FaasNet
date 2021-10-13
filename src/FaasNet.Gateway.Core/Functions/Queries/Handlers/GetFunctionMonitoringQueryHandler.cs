using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Factories;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Queries.Handlers
{
    public class GetFunctionMonitoringQueryHandler : IRequestHandler<GetFunctionMonitoringQuery, JObject>
    {
        private readonly IFunctionQueryRepository _functionQueryRepository;
        private readonly GatewayConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetFunctionMonitoringQueryHandler(
            IFunctionQueryRepository functionQueryRepository,
            IOptions<GatewayConfiguration> configuration,
            IHttpClientFactory httpClientFactory)
        {
            _functionQueryRepository = functionQueryRepository;
            _configuration = configuration.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<JObject> Handle(GetFunctionMonitoringQuery request, CancellationToken cancellationToken)
        {
            var fn = await _functionQueryRepository.Get(request.FuncName, cancellationToken);
            if (fn == null)
            {
                throw new FunctionNotFoundException(string.Format(Global.UnknownFunction, request.FuncName));
            }

            using (var httpClient = _httpClientFactory.Build())
            {
                var range = request.IsRange ? "query_range" : "query";
                var httpResponse = await httpClient.GetAsync($"{_configuration.PromotheusApi}/api/v1/{range}?{request.Query}", cancellationToken);
                httpResponse.EnsureSuccessStatusCode();
                return JObject.Parse(await httpResponse.Content.ReadAsStringAsync());
            }
        }
    }
}
