using System.Collections.Generic;

namespace FaasNet.Gateway.Core.ApiDefinitions.Queries.Results
{
    public class ApiDefinitionOperationBlockResult
    {
        public int Id { get; set; }
        public int Parent { get; set; }
        public IEnumerable<ApiDefinitionOperationBlockDataResult> Data { get; set; }
        public ApiDefinitionOperationBlockModelResult Model { get; set; }
    }
}
