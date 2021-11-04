﻿using FaasNet.Runtime.Exceptions;
using FaasNet.Runtime.OpenAPI.Models;
using FaasNet.Runtime.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.OpenAPI
{
    public class OpenAPIParser : IOpenAPIParser
    {
        private readonly Factories.IHttpClientFactory _httpClientFactory;

        public OpenAPIParser(Factories.IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<JObject> Invoke(string url, string operationId, JObject input, CancellationToken cancellationToken)
        {
            using (var httpClient = _httpClientFactory.Build())
            {
                var httpResult = await httpClient.GetAsync(url, cancellationToken);
                httpResult.EnsureSuccessStatusCode();
                var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
                var openApiResult = JsonConvert.DeserializeObject<OpenApiResult>(json);
                var httpRequestMessage = BuildHttpRequestMessage(url, openApiResult, operationId, input);
                httpResult = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                httpResult.EnsureSuccessStatusCode();
                json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
                return JObject.Parse(json);
            }
        }

        protected virtual HttpRequestMessage BuildHttpRequestMessage(string url, OpenApiResult openApiResult, string operationId, JObject input)
        {
            var path = openApiResult.Paths.FirstOrDefault(p => p.Value.Any(v => v.Value.OperationId == operationId));
            if (string.IsNullOrWhiteSpace(path.Key))
            {
                throw new OpenAPIException(string.Format(Global.UnknownOpenAPIOperation, operationId));
            }

            var operation = path.Value.First(v => v.Value.OperationId == operationId).Value;
            var uri = new Uri(url);
            var baseUrl = uri.AbsoluteUri.Replace(uri.AbsolutePath, string.Empty);
            var operationUrl = ConvertUrl($"{baseUrl}{path.Key}", input, operation);
            return new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(operationUrl)
            };
        }

        protected string ConvertUrl(string operationUrl, JObject input, OpenApiOperationResult openApiOperationResult)
        {
            var result = operationUrl;
            var missingRequiredParameters = new List<string>();
            foreach(var parameter in openApiOperationResult.Parameters)
            {
                var containsKey = input.ContainsKey(parameter.Name);
                if (parameter.Required && !containsKey)
                {
                    missingRequiredParameters.Add(parameter.Name);
                    continue;
                }

                var value = input[parameter.Name].ToString();
                result = result.Replace("{" + parameter.Name + "}", value);
            }

            if (missingRequiredParameters.Any())
            {
                throw new OpenAPIException(string.Format(Global.MissingRequiredParameters, string.Join(',', missingRequiredParameters)));
            }

            return result;
        }
    }
}