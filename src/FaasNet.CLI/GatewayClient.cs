using FaasNet.CLI.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;

namespace FaasNet.CLI
{
    public class GatewayClient
    {
        public PublishFunctionResult PublishFunction(string baseUrl, string name, string image, string version)
        {
            using (var httpClient = new HttpClient())
            {
                var cmd = new PublishFunction
                {
                    Image = image,
                    Name = name,
                    Version = version
                };
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/functions"),
                    Method = HttpMethod.Post,
                    Content = new StringContent(JsonConvert.SerializeObject(cmd), Encoding.UTF8, "application/json")
                };
                var httpResult = httpClient.SendAsync(requestMessage).Result;
                httpResult.EnsureSuccessStatusCode();
                var json = httpResult.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<PublishFunctionResult>(json);
            }
        }

        public void UnpublishFunction(string baseUrl, string name)
        {
            using (var httpClient = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/functions/{name}"),
                    Method = HttpMethod.Delete
                };
                var httpResult = httpClient.SendAsync(requestMessage).Result;
                httpResult.EnsureSuccessStatusCode();
            }
        }

        public JObject InvokeFunction(string baseUrl, string id, string configuration, string input)
        {
            using (var httpClient = new HttpClient())
            {
                var req = new JObject();
                req.Add("input", JObject.Parse(input));
                req.Add("configuration", JObject.Parse(configuration));
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/functions/{id}/invoke"),
                    Method = HttpMethod.Post,
                    Content = new StringContent(req.ToString(), Encoding.UTF8, "application/json")
                };
                var httpResult = httpClient.SendAsync(requestMessage).Result;
                httpResult.EnsureSuccessStatusCode();
                return JObject.Parse(httpResult.Content.ReadAsStringAsync().Result);
            }
        }

        public JObject GetConfiguration(string baseUrl, string name)
        {
            using (var httpClient = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/functions/{name}/configuration"),
                    Method = HttpMethod.Get
                };
                var httpResult = httpClient.SendAsync(requestMessage).Result;
                httpResult.EnsureSuccessStatusCode();
                return JObject.Parse(httpResult.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
