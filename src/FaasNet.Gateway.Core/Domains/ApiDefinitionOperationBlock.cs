using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Gateway.Core.Domains
{
    public class ApiDefinitionOperationBlock : ICloneable
    {
        public ApiDefinitionOperationBlock()
        {
            Data = new List<ApiDefinitionOperationBlockData>();
        }

        public int ExternalId { get; set; }
        public int Parent { get; set; }
        public ApiDefinitionOperationBlockModel Model { get; set; }
        public ICollection<ApiDefinitionOperationBlockData> Data { get; set; }

        public object Clone()
        {
            return new ApiDefinitionOperationBlock
            {
                ExternalId = ExternalId,
                Parent = Parent,
                Model = (ApiDefinitionOperationBlockModel)Model?.Clone(),
                Data = Data.Select(d => (ApiDefinitionOperationBlockData)d.Clone()).ToList()
            };
        }
    }
}
