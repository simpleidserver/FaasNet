using FaasNet.Runtime.JSchemas;
using FaasNet.Runtime.OpenAPI.v3.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FaasNet.Runtime.OpenAPI.v3
{
    public class OpenAPIConfigurationParser : IOpenAPIConfigurationParser
    {
        public string VersionPath => "openapi";
        public IEnumerable<string> SupportedVersions => new string[]
        {
            "3.0.3",
            "3.0.2",
            "3.0.1",
            "3.0.0"
        };

        public OpenApiResult Deserialize(string json)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceResolverProvider = () => new FaasNetReferenceResolver()
            };
            var result = JsonConvert.DeserializeObject<OpenApiResult>(json, settings);
            return result;
        }
    }
}
