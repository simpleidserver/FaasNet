using FaasNet.Gateway.Core.Domains;
using System;

namespace FaasNet.Gateway.Core.ApiDefinitions.Queries.Results
{
    public class ApiDefinitionOperationResult
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public ApiDefinitionOperationUIResult UI { get; set; }

        public static ApiDefinitionOperationResult ToDto(ApiDefinitionOperation op)
        {
            if (op == null)
            {
                return null;
            }

            return new ApiDefinitionOperationResult
            {
                CreateDateTime = op.CreateDateTime,
                Name = op.Name,
                Path = op.Path,
                UpdateDateTime = op.UpdateDateTime,
                UI = ApiDefinitionOperationUIResult.ToDto(op.UI)
            };
        }
    }
}
