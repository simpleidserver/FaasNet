using FaasNet.Gateway.Core.Factories;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Commands.Handlers
{
    public class PublishFunctionCommandHandler : IRequestHandler<PublishFunctionCommand, bool>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GatewayConfiguration _configuration;

        public PublishFunctionCommandHandler(
            IHttpClientFactory httpClientFactory,
            IOptions<GatewayConfiguration> configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration.Value;
        }

        public async Task<bool> Handle(PublishFunctionCommand command, CancellationToken cancellationToken)
        {
            using (var httpClient = _httpClientFactory.Build())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_configuration.FunctionApi}/functions"),
                    Method = HttpMethod.Post,
                    Content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json")
                };
                var httpResult = await httpClient.SendAsync(request);
                httpResult.EnsureSuccessStatusCode();
                return true;
            }
        }
    }
}
