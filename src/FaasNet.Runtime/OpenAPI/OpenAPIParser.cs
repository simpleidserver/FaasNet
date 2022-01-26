using FaasNet.Runtime.Exceptions;
using FaasNet.Runtime.OpenAPI.Builders;
using FaasNet.Runtime.OpenAPI.v3.Models;
using FaasNet.Runtime.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace FaasNet.Runtime.OpenAPI
{
    public class OpenAPIParser : IOpenAPIParser
    {
        private static Dictionary<string, HttpMethod> MAPPING_NAME_TO_HTTPMETHOD = new Dictionary<string, HttpMethod>
        {
            { "get", HttpMethod.Get },
            { "post", HttpMethod.Post },
            { "put", HttpMethod.Put },
            { "delete", HttpMethod.Delete },
            { "patch", HttpMethod.Patch }
        };
        private readonly IEnumerable<IRequestBodyBuilder> _requestBodyBuilders;
        private readonly Factories.IHttpClientFactory _httpClientFactory;
        private readonly IEnumerable<IOpenAPIConfigurationParser> _openAPIConfigurationParsers;

        public OpenAPIParser(
            Factories.IHttpClientFactory httpClientFactory,
            IEnumerable<IRequestBodyBuilder> requestBodyBuilders,
            IEnumerable<IOpenAPIConfigurationParser> openAPIConfigurationParsers)
        {
            _httpClientFactory = httpClientFactory;
            _requestBodyBuilders = requestBodyBuilders;
            _openAPIConfigurationParsers = openAPIConfigurationParsers;
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

        public Task<OpenApiResult> GetConfiguration(string url, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.Build();
            return GetConfiguration(url, httpClient, cancellationToken);
        }

        public async Task<JToken> Invoke(string url, string operationId, JToken input, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.Build();
            var openApiResult = await GetConfiguration(url, httpClient, cancellationToken);
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

        protected virtual async Task<OpenApiResult> GetConfiguration(string url, HttpClient httpClient, CancellationToken cancellationToken)
        {
            var httpResult = await httpClient.GetAsync(url, cancellationToken);
            var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            var jObj = JObject.Parse(json);
            var parser = _openAPIConfigurationParsers.FirstOrDefault(p =>
            {
                var version = jObj.SelectToken(p.VersionPath);
                if (version == null)
                {
                    return false;
                }

                return p.SupportedVersions.Contains(version.ToString());
            });
            if (parser == null)
            {
                throw new OpenAPIException(Global.UnsupportedOpenApiVersion);
            }

            return parser.Deserialize(json);
        }

        protected virtual HttpRequestMessage BuildHttpRequestMessage(string url, OpenApiResult openApiResult, string operationId, JToken input)
        {
            const string queriesName = "queries";
            const string propertiesName = "properties";
            var path = openApiResult.Paths.FirstOrDefault(p => p.Value.Any(v => v.Value.OperationId == operationId));
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
            var operation = path.Value.First(v => v.Value.OperationId == operationId).Value;
            var uri = new Uri(url);
            var baseUrl = uri.AbsoluteUri.Replace(uri.AbsolutePath, string.Empty);
            if (openApiResult.Servers != null && openApiResult.Servers.Any())
            {
                baseUrl = baseUrl + openApiResult.Servers.First().Url;
            }

            var validationErrors = new List<string>();
            var queries = input.SelectToken(queriesName) ?? input;
            var properties = input.SelectToken(propertiesName);
            var operationUrl = BuildHTTPTarget($"{baseUrl}{path.Key}", queries, operation, validationErrors);
            var content = BuildHTTPContent(properties, operation, openApiResult.Components, validationErrors);
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

        protected string BuildHTTPTarget(string operationUrl, JToken input, OpenApiOperationResult openApiOperationResult, List<string> validationErrors)
        {
            var result = operationUrl;
            var jObj = input as JObject;
            var queryParameters = new Dictionary<string, string>();
            if (openApiOperationResult.Parameters == null)
            {
                return result;
            }

            foreach (var parameter in openApiOperationResult.Parameters)
            {
                JToken token = null;
                if (input.Type == JTokenType.Object && parameter.Required && (token = jObj.SelectToken(parameter.Name)) == null)
                {
                    validationErrors.Add(string.Format(Global.MissingPropertyFromInput, parameter.Name));
                    continue;
                }

                if (parameter.In == "path")
                {
                    switch (input.Type)
                    {
                        case JTokenType.String:
                            result = result.Replace("{" + parameter.Name + "}", HttpUtility.UrlEncode(input.ToString()));
                            break;
                        case JTokenType.Object:
                            result = result.Replace("{" + parameter.Name + "}", HttpUtility.UrlEncode(token.ToString()));
                            break;
                    }
                }
                else if (parameter.In == "query")
                {
                    switch (input.Type)
                    {
                        case JTokenType.String:
                            queryParameters.Add(parameter.Name, input.ToString());
                            break;
                        case JTokenType.Object:
                            queryParameters.Add(parameter.Name, token.ToString());
                            break;
                    }
                }
                else
                {
                    throw new OpenAPIException(string.Format(Global.UnsupportedInParameter, parameter.In));
                }
            }

            if (queryParameters.Any())
            {
                result = $"{result}?{string.Join('&', queryParameters.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";
            }

            return result;
        }

        protected HttpContent BuildHTTPContent(JToken input, OpenApiOperationResult openApiOperationResult, OpenApiComponentsSchemaResult components, List<string> validationErrors)
        {
            var requestBody = openApiOperationResult.RequestBody;
            if (requestBody == null || input == null)
            {
                return null;
            }

            var content = requestBody.Content.FirstOrDefault(kvp => _requestBodyBuilders.Any(r => r.ContentTypes.Contains(kvp.Key)));
            if(content.Equals(default(KeyValuePair<string, OpenApiRequestBodySchemaResult>)) || content.Value == null)
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
}