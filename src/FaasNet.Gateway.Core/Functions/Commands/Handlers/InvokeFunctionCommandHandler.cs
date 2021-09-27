using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Factories;
using FaasNet.Gateway.Core.Parameters;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Commands.Handlers
{
    public class InvokeFunctionCommandHandler : IRequestHandler<InvokeFunctionCommand, JObject>
    {
        private readonly IFunctionRepository _functionRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GatewayConfiguration _configuration;

        public InvokeFunctionCommandHandler(
            IFunctionRepository functionRepository,
            IHttpClientFactory httpClientFactory,
            IOptions<GatewayConfiguration> configuration)
        {
            _functionRepository = functionRepository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration.Value;
        }

        public async Task<JObject> Handle(InvokeFunctionCommand request, CancellationToken cancellationToken)
        {
            var function = await _functionRepository.Get(request.FuncName, cancellationToken);
            if (function == null)
            {
                throw new FunctionNotFoundException(string.Format(Global.UnknownFunction, request.FuncName));
            }

            var parameter = new InvokeFunctionParameter
            {
                Name = request.FuncName,
                Content = new FunctionParameter
                {
                    Configuration = request.Configuration,
                    Input = request.Input
                }
            };
            using (var httpClient = _httpClientFactory.Build())
            {
                var httpRequest = new HttpRequestMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(parameter), Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{_configuration.FunctionApi}/functions/invoke")
                };
                var httpResult = await httpClient.SendAsync(httpRequest);
                httpResult.EnsureSuccessStatusCode();
                var str = await httpResult.Content.ReadAsStringAsync();
                return JObject.Parse(str);
            }
        }
    }
}
