using FaasNet.StateMachine.Runtime.Exceptions;
using FaasNet.StateMachine.Runtime.Resources;
using Newtonsoft.Json.Linq;
using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace FaasNet.StateMachine.Runtime.OpenAPI
{
    public interface IOpenAPIParser
    {
        bool TryParseUrl(string url, out OpenAPIUrlResult result);
        Task<OpenApiDocument> GetConfiguration(string url, CancellationToken cancellationToken);
        Task<JToken> Invoke(string url, string operationId, JToken input, CancellationToken cancellationToken);
    }

    public class OpenAPIParser : IOpenAPIParser
    {
        private static Dictionary<string, HttpMethod> MAPPING_NAME_TO_HTTPMETHOD = new Dictionary<string, HttpMethod>
        {
            { OpenApiOperationMethod.Get, HttpMethod.Get },
            { OpenApiOperationMethod.Post, HttpMethod.Post },
            { OpenApiOperationMethod.Put, HttpMethod.Put },
            { OpenApiOperationMethod.Delete, HttpMethod.Delete },
            { OpenApiOperationMethod.Patch, HttpMethod.Patch }
        };
        private readonly Factories.IHttpClientFactory _httpClientFactory;
        private readonly IEnumerable<IRequestBodyBuilder> _requestBodyBuilders;

        public OpenAPIParser(Factories.IHttpClientFactory httpClientFactory, IEnumerable<IRequestBodyBuilder> requestBodyBuilders)
        {
            _httpClientFactory = httpClientFactory;
            _requestBodyBuilders = requestBodyBuilders;
        }

        public bool TryParseUrl(string url, out OpenAPIUrlResult result)
        {
            var splitted = url.Split('#');
            if (splitted.Count() != 2)
            {
                result = null;
                return false;
            }

            result = new OpenAPIUrlResult(splitted.First(), splitted.Last());
            return true;
        }

        public async Task<OpenApiDocument> GetConfiguration(string url, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.Build();
            var json = await httpClient.GetStringAsync(url, cancellationToken);
            var openApiDocument = await OpenApiDocument.FromJsonAsync(json, cancellationToken);
            return openApiDocument;
        }

        public async Task<JToken> Invoke(string url, string operationId, JToken input, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.Build();
            var openApiResult = await GetConfiguration(url, cancellationToken);
            var httpRequestMessage = BuildHttpRequestMessage(url, openApiResult, operationId, input);
            var httpResult = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
            httpResult.EnsureSuccessStatusCode();
            var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            try
            {
                return JToken.Parse(json);
            }
            catch
            {
                return json;
            }
        }

        protected virtual HttpRequestMessage BuildHttpRequestMessage(string url, OpenApiDocument openApiDocument, string operationId, JToken input)
        {
            const string queriesName = "queries";
            const string propertiesName = "properties";
            KeyValuePair<string, OpenApiPathItem> path = openApiDocument.Paths.FirstOrDefault(p => p.Value.Any(v => v.Value.OperationId == operationId));
            if (string.IsNullOrWhiteSpace(path.Key))
            {
                throw new OpenAPIException(string.Format(Global.UnknownOpenAPIOperation, operationId));
            }

            var kvp = path.Value.First(v => v.Value.OperationId == operationId);
            var httpOperation = kvp.Key;
            if (!MAPPING_NAME_TO_HTTPMETHOD.Any(kvp => kvp.Key.Equals(httpOperation, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new OpenAPIException(string.Format(Global.UnknownHttpAPIOperation, httpOperation));
            }

            var httpMethod = MAPPING_NAME_TO_HTTPMETHOD.First(kvp => kvp.Key.Equals(httpOperation, StringComparison.InvariantCultureIgnoreCase)).Value;
            OpenApiOperation operation = path.Value.First(v => v.Value.OperationId == operationId).Value;
            var uri = new Uri(url);
            var baseUrl = uri.AbsoluteUri.Replace(uri.AbsolutePath, string.Empty);
            if (openApiDocument.Servers != null && openApiDocument.Servers.Any())
            {
                baseUrl = baseUrl + openApiDocument.Servers.First().Url;
            }

            var validationErrors = new List<string>();
            var queries = input.SelectToken(queriesName) ?? input;
            var properties = input.SelectToken(propertiesName);
            var operationUrl = BuildHTTPTarget($"{baseUrl}{path.Key}", queries, operation, validationErrors);
            var content = BuildHTTPContent(properties, operation, openApiDocument.Components, validationErrors);
            if(validationErrors.Any())
            {
                throw new OpenAPIException(string.Join(',', validationErrors));
            }

            return new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = new Uri(operationUrl),
                Content = content
            };
        }

        protected string BuildHTTPTarget(string operationUrl, JToken input, OpenApiOperation openApiOperationResult, List<string> validationErrors)
        {
            var result = operationUrl;
            var jObj = input as JObject;
            var queryParameters = new Dictionary<string, string>();
            if (openApiOperationResult.Parameters == null)
            {
                return result;
            }

            foreach (var parameter in openApiOperationResult.Parameters.Where(p => p.Kind == OpenApiParameterKind.Path || p.Kind == OpenApiParameterKind.Query))
            {
                var val = input.SelectToken(parameter.Name) ?? string.Empty;
                var errors = parameter.Schema.Validate(val);
                if (errors.Any())
                {
                    validationErrors.AddRange(errors.Select(e => e.ToString()));
                    continue;
                }

                switch (parameter.Kind)
                {
                    case OpenApiParameterKind.Path:
                        switch (input.Type)
                        {
                            case JTokenType.String:
                                result = result.Replace("{" + parameter.Name + "}", HttpUtility.UrlEncode(input.ToString()));
                                break;
                            case JTokenType.Object:
                                result = result.Replace("{" + parameter.Name + "}", HttpUtility.UrlEncode(input.ToString()));
                                break;
                        }
                        break;
                    case OpenApiParameterKind.Query:
                        switch (input.Type)
                        {
                            case JTokenType.String:
                                queryParameters.Add(parameter.Name, val.ToString());
                                break;
                            case JTokenType.Object:
                                queryParameters.Add(parameter.Name, input.ToString());
                                break;
                        }
                        break;
                }
            }

            if (queryParameters.Any())
            {
                result = $"{result}?{string.Join('&', queryParameters.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";
            }

            return result;
        }

        protected HttpContent BuildHTTPContent(JToken input, OpenApiOperation openApiOperationResult, OpenApiComponents components, List<string> validationErrors)
        {
            var requestBody = openApiOperationResult.RequestBody;
            if (requestBody == null || input == null)
            {
                return null;
            }

            var content = requestBody.Content.FirstOrDefault(kvp => _requestBodyBuilders.Any(r => r.ContentTypes.Contains(kvp.Key)));
            if(content.Equals(default(KeyValuePair<string, OpenApiMediaType>)) || content.Value == null)
            {
                throw new OpenAPIException(string.Format(Global.NoContentTypeSupportedInOperation, openApiOperationResult.OperationId));
            }

            var bodyBuilder = _requestBodyBuilders.First(r => r.ContentTypes.Contains(content.Key));
            if (content.Value.Schema == null)
            {
                throw new OpenAPIException(string.Format(Global.NoSchemaSpecifiedInContentType, $"{openApiOperationResult.OperationId}/{content.Key}"));
            }

            var schema = content.Value.Schema;
            var validation = schema.Validate(input.ToString());
            if(validation.Any())
            {
                validationErrors.AddRange(validation.Select(v => $"{v.Kind.ToString()}:{v.Path}").ToList());
            }

            return bodyBuilder.Build(content.Key, input);
        }
    }

    public record OpenAPIUrlResult
    {
        public OpenAPIUrlResult(string url, string operationId)
        {
            Url = url;
            OperationId = operationId;
        }

        public string Url { get; set; }
        public string OperationId { get; set; }
    }
}