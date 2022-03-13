using FaasNet.StateMachine.Runtime.OpenAPI.v3.Models;
using System.Collections.Generic;

namespace FaasNet.StateMachine.Runtime.OpenAPI
{
    public interface IOpenAPIConfigurationParser
    {
        string VersionPath { get; }
        IEnumerable<string> SupportedVersions { get; }
        OpenApiResult Deserialize(string json);
    }
}
