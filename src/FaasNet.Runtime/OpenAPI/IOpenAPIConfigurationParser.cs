using FaasNet.Runtime.OpenAPI.Models;
using System.Collections.Generic;

namespace FaasNet.Runtime.OpenAPI
{
    public interface IOpenAPIConfigurationParser
    {
        string VersionPath { get; }
        IEnumerable<string> SupportedVersions { get; }
        OpenApiResult Deserialize(string json);
    }
}
