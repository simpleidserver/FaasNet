using System;

namespace FaasNet.Gateway.Core.Domains
{
    public class ApiDefinitionOperationBlockModelInfo : ICloneable
    {
        public string Name { get; set; }

        public object Clone()
        {
            return new ApiDefinitionOperationBlockModelInfo
            {
                Name = Name
            };
        }
    }
}
