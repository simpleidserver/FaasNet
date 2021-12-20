using FaasNet.Gateway.Core.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Invokers
{
    public class KubernetesFunctionInvoker : IFunctionInvoker
    {
        private readonly Runtime.Factories.IHttpClientFactory _httpClientFactory;
        private readonly GatewayConfiguration _configuration;
        private readonly IPrometheusHelper _promotheusHelper;

        public KubernetesFunctionInvoker(Runtime.Factories.IHttpClientFactory httpClientFactory, IOptions<GatewayConfiguration> configuration, IPrometheusHelper promotheusHelper)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration.Value;
            _promotheusHelper = promotheusHelper;
        }

        public async Task Publish(string id, string image, string version, string command, CancellationToken cancellationToken)
        {
            using (var httpClient = _httpClientFactory.Build())
            {
                var json = JsonConvert.SerializeObject(new
                {
                    Image = image,
                    Id = id,
                    Version = version
                });
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_configuration.FunctionApi}/functions"),
                    Method = HttpMethod.Post,
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                var httpResult = await httpClient.SendAsync(request, cancellationToken);
                httpResult.EnsureSuccessStatusCode();
            }
        }

        public Task InitAudit(string id, CancellationToken cancellationToken)
        {
            _promotheusHelper.Add(id);
            return Task.CompletedTask;
        }

        public async Task<JToken> Invoke(string id, JToken input, JObject configuration, CancellationToken cancellationToken)
        {
            using (var httpClient = _httpClientFactory.Build())
            {
                var json = JsonConvert.SerializeObject(new KubernetesInvokeFunctionParameter
                {
                    Id = id,
                    Content = new FunctionParameter
                    {
                        Configuration = configuration,
                        Input = input
                    }
                });
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_configuration.FunctionApi}/functions/invoke"),
                    Method = HttpMethod.Post,
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                var httpResult = await httpClient.SendAsync(request, cancellationToken);
                httpResult.EnsureSuccessStatusCode();
                var str = await httpResult.Content.ReadAsStringAsync();
                return JToken.Parse(str);
            }
        }

        public async Task Unpublish(string id, CancellationToken cancellationToken)
        {
            using (var httpClient = _httpClientFactory.Build())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_configuration.FunctionApi}/functions/{id}"),
                    Method = HttpMethod.Delete
                };
                var httpResult = await httpClient.SendAsync(request);
                httpResult.EnsureSuccessStatusCode();
            }
        }

        public Task RemoveAudit(string id, CancellationToken cancellationToken)
        {
            _promotheusHelper.Remove(id);
            return Task.CompletedTask;
        }

        private class KubernetesPublishFunctionParameter
        {
            public string Image { get; set; }
        }

        private class KubernetesInvokeFunctionParameter
        {
            public string Id { get; set; }
            public FunctionParameter Content { get; set; }
        }

        private class FunctionParameter
        {
            public JToken Input { get; set; }
            public JObject Configuration { get; set; }
        }
    }
}
