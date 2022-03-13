using FaasNet.Domain.Exceptions;
using FaasNet.Function.Core.Factories;
using FaasNet.Function.Core.Repositories;
using FaasNet.Function.Core.Resources;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Function.Core.Functions.Queries.Handlers
{
    public class GetFunctionMonitoringQueryHandler : IRequestHandler<GetFunctionMonitoringQuery, JObject>
    {
        private readonly IFunctionRepository _functionRepository;
        private readonly FunctionOptions _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetFunctionMonitoringQueryHandler(IFunctionRepository functionRepository, IOptions<FunctionOptions> configuration, IHttpClientFactory httpClientFactory)
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
                throw new NotFoundException(ErrorCodes.UNKNOWN_FUNCTION, string.Format(Global.UnknownFunction, request.Id));
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
