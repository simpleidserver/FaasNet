using Newtonsoft.Json.Linq;
using System;

namespace FaasNet.Gateway.Core.ApiDefinitions.Queries.Results
{
    public class ApiDefinitionOperationBlockModelResult : ICloneable
    {
        public JObject Configuration { get; set; }
        public ApiDefinitionOperationBlockModelInfoResult Info { get; set; }

        public object Clone()
        {
            return new ApiDefinitionOperationBlockModelResult
            {
                Configuration = Configuration,
                Info = (ApiDefinitionOperationBlockModelInfoResult)Info?.Clone()
            };
        }
    }
}
