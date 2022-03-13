using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.Clients
{
    public class FunctionService : IFunctionService
    {
        private readonly StateMachineOptions _options;

        public FunctionService(IOptions<StateMachineOptions> options)
        {
            _options = options.Value;
        }

        public async Task<FunctionResult> Get(string image, string version, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{_options.FunctionApiBaseUrl}/functions/{image}/{version}")
                };
                var httpResult = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                httpResult.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<FunctionResult>(await httpResult.Content.ReadAsStringAsync(cancellationToken));
            }
        }

        public async Task<string> Publish(FunctionResult functionResult, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                var json = JObject.FromObject(functionResult).ToString();
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{_options.FunctionApiBaseUrl}/functions"),
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                var httpResult = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                httpResult.EnsureSuccessStatusCode();
                return JObject.Parse((await httpResult.Content.ReadAsStringAsync(cancellationToken)))["id"].ToString();
            }
        }
    }
}
