using FaasNet.Runtime.Exceptions;
using FaasNet.Runtime.OpenAPI.Builders;
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

        public OpenAPIParser(
            Factories.IHttpClientFactory httpClientFactory,
            IEnumerable<IRequestBodyBuilder> requestBodyBuilders)
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
            var settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore
            };
            return JsonConvert.DeserializeObject<OpenApiResult>(json, settings);
        }

        protected virtual HttpRequestMessage BuildHttpRequestMessage(string url, OpenApiResult openApiResult, string operationId, JToken input)
        {
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
            if (openApiResult.Schemes != null && openApiResult.Schemes.Any() && !string.IsNullOrWhiteSpace(openApiResult.Host))
            {
                baseUrl = $"{openApiResult.Schemes.First()}://{openApiResult.Host}";
                if (!string.IsNullOrWhiteSpace(openApiResult.BasePath))
                {
                    baseUrl = $"{baseUrl}{openApiResult.BasePath}";
                }
            }

            var validationErrors = new List<string>();
            var operationUrl = BuildHTTPTarget($"{baseUrl}{path.Key}", input, operation, validationErrors);
            var content = BuildHTTPContent(input, operation, openApiResult.Components, validationErrors);
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

            foreach(var parameter in openApiOperationResult.Parameters)
            {
                JToken token = null;
                if (input.Type == JTokenType.Object && parameter.Required && (token = jObj.SelectToken(parameter.Name)) == null)
                {
                    validationErrors.Add(string.Format(Global.MissingPropertyFromInput, parameter.Name));
                    continue;
                }

                if(parameter.In == "path")
                {
                    switch(input.Type)
                    {
                        case JTokenType.String:
                            result = result.Replace("{" + parameter.Name + "}", HttpUtility.UrlEncode(input.ToString()));
                            break;
                        case JTokenType.Object:
                            result = result.Replace("{" + parameter.Name + "}", HttpUtility.UrlEncode(token.ToString()));
                            break;
                    }
                }
                else if(parameter.In == "query")
                {
                    switch(input.Type)
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
            if (requestBody == null)
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

            var schemaReference = content.Value.Schema.Ref.Split("/").Last();
            if (!components.Schemas.ContainsKey(schemaReference))
            {
                throw new OpenAPIException(string.Format(Global.UnknownSchema, schemaReference));
            }

            var schema = components.Schemas.First(s => s.Key == schemaReference).Value;
            var inputToken = Parse(input, schema, components, validationErrors);
            return bodyBuilder.Build(content.Key, inputToken);
        }

        #region Build Input

        private static JToken Parse(JToken input, OpenApiSchemaResult openApiSchema, OpenApiComponentsSchemaResult components, List<string> validationErrors, string parentPath = null)
        {
            JToken result = null;
            if (openApiSchema.Type == "object")
            {
                result = new JObject();
            }
            else if (openApiSchema.Type == "array")
            {
                result = new JArray();
            }

            foreach (var property in openApiSchema.Properties)
            {
                Parse(input, result, parentPath, property.Key, property.Value, openApiSchema.Required, components, validationErrors);
            }

            return result;
        }

        private static bool Parse(JToken input, JToken parent, string parentPath, string propertyName, OpenApiSchemaPropertyResult propertyResult, IEnumerable<string> required, OpenApiComponentsSchemaResult components, List<string> validationErrors)
        {
            Func<JToken, string, List<string>, string, JToken> callback = (inpt, reference, errs, fp) =>
            {
                var record = new JObject();
                var schemaReference = reference.Split("/").Last();
                if (!components.Schemas.ContainsKey(schemaReference))
                {
                    errs.Add(string.Format(Global.UnknownSchema, schemaReference));
                    return null;
                }

                var schema = components.Schemas.First(s => s.Key == schemaReference).Value;
                return Parse(inpt, schema, components, validationErrors, fp);
            };

            var nullable = propertyResult.Nullable;
            var fullPath = string.IsNullOrWhiteSpace(parentPath) ? propertyName : $"{parentPath}.{propertyName}";
            var token = input.SelectToken(fullPath);
            if ((required != null && required.Contains(propertyName)) && token == null)
            {
                validationErrors.Add(string.Format(Global.MissingPropertyFromInput, fullPath));
                return false;
            }

            if (token == null)
            {
                return true;
            }

            switch (propertyResult.Type)
            {
                case "string":
                    if(token.Type != JTokenType.String)
                    {
                        validationErrors.Add(string.Format(Global.InvalidStringInput, fullPath));
                        return false;
                    }

                    Add(parent, new JProperty(propertyName, token));
                    break;
                case "array":
                    if (token.Type != JTokenType.Array)
                    {
                        validationErrors.Add(string.Format(Global.InvalidArrayInput, fullPath));
                        return false;
                    }

                    var jArr = new JArray();
                    var tokens = token as JArray;
                    foreach(var child in tokens)
                    {
                        if (string.IsNullOrWhiteSpace(propertyResult.Items.Reference))
                        {
                            jArr.Add(child);
                            continue;
                        }

                        var r = callback(child, propertyResult.Items.Reference, validationErrors, string.Empty);
                        if (r == null)
                        {
                            return false;
                        }

                        jArr.Add(r);
                    }

                    Add(parent, new JProperty(propertyName, tokens));
                    break;
                default:
                    if (string.IsNullOrWhiteSpace(propertyResult.Reference))
                    {
                        validationErrors.Add(string.Format(Global.UnsupportedType, propertyResult.Type));
                        return false;
                    }

                    var inputToken = callback(input, propertyResult.Reference, validationErrors, fullPath);
                    if (inputToken == null)
                    {
                        return false;
                    }

                    Add(parent, new JProperty(propertyName, inputToken));
                    break;
            }

            return true;
        }

        private static void Add(JToken parent, JToken value)
        {
            switch (parent.Type)
            {
                case JTokenType.Array:
                    var jArr = parent as JArray;
                    jArr.Add(value);
                    break;
                case JTokenType.Object:
                    var jObj = parent as JObject;
                    jObj.Add(value);
                    break;
            }
        }

        #endregion
    }
}