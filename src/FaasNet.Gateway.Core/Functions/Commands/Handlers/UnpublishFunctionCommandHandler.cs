using FaasNet.Gateway.Core.Factories;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Commands.Handlers
{
    public class UnpublishFunctionCommandHandler : IRequestHandler<UnpublishFunctionCommand, bool>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GatewayConfiguration _configuration;

        public UnpublishFunctionCommandHandler(
            IHttpClientFactory httpClientFactory,
            IOptions<GatewayConfiguration> configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration.Value;
        }

        public async Task<bool> Handle(UnpublishFunctionCommand command, CancellationToken cancellationToken)
        {
            using (var httpClient = _httpClientFactory.Build())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_configuration.FunctionApi}/functions/{command.Name}"),
                    Method = HttpMethod.Delete
                };
                var httpResult = await httpClient.SendAsync(request);
                httpResult.EnsureSuccessStatusCode();
                return true;
            }
        }
    }
}
