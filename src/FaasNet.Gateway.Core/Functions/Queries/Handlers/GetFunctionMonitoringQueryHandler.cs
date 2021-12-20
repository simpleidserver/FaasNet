using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
using FaasNet.Runtime.Factories;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Queries.Handlers
{
    public class GetFunctionMonitoringQueryHandler : IRequestHandler<GetFunctionMonitoringQuery, JObject>
    {
        private readonly IFunctionRepository _functionRepository;
        private readonly GatewayConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetFunctionMonitoringQueryHandler(
            IFunctionRepository functionRepository,
            IOptions<GatewayConfiguration> configuration,
            IHttpClientFactory httpClientFactory)
        {
            _functionRepository = functionRepository;
            _configuration = configuration.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<JObject> Handle(GetFunctionMonitoringQuery request, CancellationToken cancellationToken)
        {
            var fn = _functionRepository.Query().FirstOrDefault(r => r.Id == request.Id);
            if (fn == null)
            {
                throw new FunctionNotFoundException(ErrorCodes.UnknownFunction, string.Format(Global.UnknownFunction, request.Id));
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
