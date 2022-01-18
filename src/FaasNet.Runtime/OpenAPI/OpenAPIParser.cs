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

        public async Task<JToken> Invoke(string url, string operationId, JToken input, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.Build();
            var httpResult = await httpClient.GetAsync(url, cancellationToken);
            httpResult.EnsureSuccessStatusCode();
            var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            var settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore
            };
            var openApiResult = JsonConvert.DeserializeObject<OpenApiResult>(json, settings);
            var httpRequestMessage = BuildHttpRequestMessage(url, openApiResult, operationId, input);
            httpResult = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
            httpResult.EnsureSuccessStatusCode();
            json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            try
            {
                return JToken.Parse(json);
            }
            catch
            {
                return json;
            }
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
            var operationUrl = BuildUrl($"{baseUrl}{path.Key}", input, operation);
            var content = BuildContent(input, operation, openApiResult.Components);
            return new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = new Uri(operationUrl),
                Content = content
            };
        }

        protected string BuildUrl(string operationUrl, JToken input, OpenApiOperationResult openApiOperationResult)
        {
            var result = operationUrl;
            var jObj = input as JObject;
            if (jObj != null)
            {
                var missingRequiredParameters = new List<string>();
                if (openApiOperationResult.Parameters != null && openApiOperationResult.Parameters.Any())
                {
                    foreach (var parameter in openApiOperationResult.Parameters)
                    {
                        var containsKey = jObj.ContainsKey(parameter.Name);
                        if (parameter.Required && !containsKey)
                        {
                            missingRequiredParameters.Add(parameter.Name);
                            continue;
                        }

                        var value = input[parameter.Name].ToString();
                        result = result.Replace("{" + parameter.Name + "}", value);
                    }
                }

                if (missingRequiredParameters.Any())
                {
                    throw new OpenAPIException(string.Format(Global.MissingRequiredParameters, string.Join(',', missingRequiredParameters)));
                }
            }
            else
            {
                foreach (var parameter in openApiOperationResult.Parameters)
                {
                    result = result.Replace("{" + parameter.Name + "}", HttpUtility.UrlEncode(input.ToString()));
                }

            }

            return result;
        }

        protected HttpContent BuildContent(JToken input, OpenApiOperationResult openApiOperationResult, OpenApiComponentsSchemaResult components)
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
            var inputToken = Parse(input, schema, components);
            return bodyBuilder.Build(content.Key, inputToken);
        }

        #region Build Input

        private static JToken Parse(JToken input, OpenApiSchemaResult openApiSchema, OpenApiComponentsSchemaResult components, string parentPath = null, List<string> validationErrors = null)
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

            if (validationErrors == null)
            {
                validationErrors = new List<string>();
            }

            foreach (var property in openApiSchema.Properties)
            {
                Parse(input, result, parentPath, property.Key, property.Value, openApiSchema.Required, components, validationErrors);
            }

            if (validationErrors.Any() && string.IsNullOrWhiteSpace(parentPath))
            {
                throw new OpenAPIException(string.Join(',', validationErrors));
            }

            return result;
        }

        private static bool Parse(JToken input, JToken parent, string parentPath, string propertyName, OpenApiSchemaPropertyResult propertyResult, IEnumerable<string> required, OpenApiComponentsSchemaResult components, List<string> validationErrors)
        {
            var nullable = propertyResult.Nullable;
            var fullPath = string.IsNullOrWhiteSpace(parentPath) ? propertyName : $"{parentPath}.{propertyName}";
            var token = input.SelectToken(fullPath);
            if ((required != null && required.Contains(propertyName)) && token == null)
            {
                validationErrors.Add(string.Format(Global.MissingPropertyFromInput, fullPath));
                return false;
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
                default:
                    if (string.IsNullOrWhiteSpace(propertyResult.Reference))
                    {
                        validationErrors.Add(string.Format(Global.UnsupportedType, propertyResult.Type));
                        return false;
                    }

                    var record = new JObject();
                    var schemaReference = propertyResult.Reference.Split("/").Last();
                    if (!components.Schemas.ContainsKey(schemaReference))
                    {
                        validationErrors.Add(string.Format(Global.UnknownSchema, schemaReference));
                        return false;
                    }

                    var schema = components.Schemas.First(s => s.Key == schemaReference).Value;
                    var inputToken = Parse(input, schema, components, fullPath, validationErrors);
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