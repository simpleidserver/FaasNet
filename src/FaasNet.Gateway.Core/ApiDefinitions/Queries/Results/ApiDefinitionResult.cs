using FaasNet.Gateway.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Gateway.Core.ApiDefinitions.Queries.Results
{
    public class ApiDefinitionResult
    {
        public ApiDefinitionResult()
        {
            Operations = new List<ApiDefinitionOperationResult>();
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public ICollection<ApiDefinitionOperationResult> Operations { get; set; }

        public static ApiDefinitionResult ToDto(ApiDefinitionAggregate apiDef)
        {
            return new ApiDefinitionResult
            {
                CreateDateTime = apiDef.CreateDateTime,
                Name = apiDef.Name,
                Path = apiDef.Path,
                UpdateDateTime = apiDef.UpdateDateTime,
                Operations = apiDef.Operations.Select(op => ApiDefinitionOperationResult.ToDto(op)).ToList()
            };
        }
    }
}
