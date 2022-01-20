using FaasNet.Runtime.OpenAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FaasNet.Runtime.OpenAPI.ThreeDotZero
{
    public class ThreeDotZeroOpenAPIConfigurationParser : IOpenAPIConfigurationParser
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
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore
            };
            return JsonConvert.DeserializeObject<OpenApiResult>(json, settings);
        }
    }
}
