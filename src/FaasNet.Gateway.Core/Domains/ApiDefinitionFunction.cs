using System;

namespace FaasNet.Gateway.Core.Domains
{
    public class ApiDefinitionFunction : ICloneable
    {
        public string Name { get; set; }
        public string Function { get; set; }
        public string SerializedConfiguration { get; set; }

        #region Operations

        public void UpdateConfiguration(string conf)
        {
            SerializedConfiguration = conf;
        }

        #endregion

        public static ApiDefinitionFunction Create(string id, string fn)
        {
            return new ApiDefinitionFunction
            {
                Name = id,
                Function = fn
            };
        }

        public object Clone()
        {
            return new ApiDefinitionFunction
            {
                Name = Name,
                Function = Function,
                SerializedConfiguration = SerializedConfiguration
            };
        }
    }
}
