using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Factories;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
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
        private readonly IFunctionCommandRepository _functionRepository;
        private readonly GatewayConfiguration _configuration;

        public UnpublishFunctionCommandHandler(
            IHttpClientFactory httpClientFactory,
            IFunctionCommandRepository functionRepository,
            IOptions<GatewayConfiguration> configuration)
        {
            _httpClientFactory = httpClientFactory;
            _functionRepository = functionRepository;
            _configuration = configuration.Value;
        }

        public async Task<bool> Handle(UnpublishFunctionCommand command, CancellationToken cancellationToken)
        {
            var function = await _functionRepository.Get(command.Name, cancellationToken);
            if (function == null)
            {
                throw new FunctionNotFoundException(string.Format(Global.UnknownFunction, command.Name));
            }

            using (var httpClient = _httpClientFactory.Build())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_configuration.FunctionApi}/functions/{command.Name}"),
                    Method = HttpMethod.Delete
                };
                var httpResult = await httpClient.SendAsync(request);
                httpResult.EnsureSuccessStatusCode();
                await _functionRepository.Delete(function, cancellationToken);
                await _functionRepository.SaveChanges(cancellationToken);
                return true;
            }
        }
    }
}
