using System;

namespace FaasNet.Gateway.Core.Domains
{
    public class ApiDefinitionOperationBlockData : ICloneable
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public object Clone()
        {
            return new ApiDefinitionOperationBlockData
            {
                Name = Name,
                Value = Value
            };
        }
    }
}
