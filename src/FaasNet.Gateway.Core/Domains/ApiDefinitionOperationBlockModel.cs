using Newtonsoft.Json.Linq;
using System;

namespace FaasNet.Gateway.Core.Domains
{
    public class ApiDefinitionOperationBlockModel : ICloneable
    {
        public JObject Configuration { get; set; }
        public ApiDefinitionOperationBlockModelInfo Info { get; set; }

        public object Clone()
        {
            return new ApiDefinitionOperationBlockModel
            {
                Configuration = Configuration,
                Info= (ApiDefinitionOperationBlockModelInfo)Info?.Clone()
            };
        }
    }
}
