using System;

namespace FaasNet.Gateway.Core.ApiDefinitions.Queries.Results
{
    public class ApiDefinitionOperationBlockModelInfoResult : ICloneable
    {
        public string Name { get; set; }

        public object Clone()
        {
            return new ApiDefinitionOperationBlockModelInfoResult
            {
                Name = Name
            };
        }
    }
}
