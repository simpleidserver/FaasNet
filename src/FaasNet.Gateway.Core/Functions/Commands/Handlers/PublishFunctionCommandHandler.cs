using FaasNet.Gateway.Core.Domains;
using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Factories;
using FaasNet.Gateway.Core.Helpers;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
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
        private readonly Factories.IHttpClientFactory _httpClientFactory;
        private readonly IFunctionCommandRepository _functionRepository;
        private readonly IPrometheusHelper _prometheusHelper;
        private readonly GatewayConfiguration _configuration;

        public PublishFunctionCommandHandler(
            Factories.IHttpClientFactory httpClientFactory,
            IFunctionCommandRepository functionRepository,
            IPrometheusHelper prometheusHelper,
            IOptions<GatewayConfiguration> configuration)
        {
            _httpClientFactory = httpClientFactory;
            _functionRepository = functionRepository;
            _prometheusHelper = prometheusHelper;
            _configuration = configuration.Value;
        }

        public async Task<bool> Handle(PublishFunctionCommand command, CancellationToken cancellationToken)
        {
            var function = await _functionRepository.Get(command.Image, cancellationToken);
            if (function != null)
            {
                throw new BusinessRuleException(string.Format(Global.FunctionNameAlreadyExists, command.Image));
            }

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
                function = FunctionAggregate.Create(command.Name, command.Image);
                await _functionRepository.Add(function, cancellationToken);
                await _functionRepository.SaveChanges(cancellationToken);
                _prometheusHelper.Add(command.Name);
                return true;
            }
        }
    }
}
