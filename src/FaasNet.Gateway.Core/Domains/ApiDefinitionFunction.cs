using System;

namespace FaasNet.Gateway.Core.Domains
{
    public class ApiDefinitionFunction : ICloneable
    {
        public string Function { get; set; }
        public string SerializedConfiguration { get; set; }

        #region Operations

        public void UpdateConfiguration(string conf)
        {
            SerializedConfiguration = conf;
        }

        #endregion

        public static ApiDefinitionFunction Create(string fn)
        {
            return new ApiDefinitionFunction
            {
                Function = fn
            };
        }

        public object Clone()
        {
            return new ApiDefinitionFunction
            {
                Function = Function,
                SerializedConfiguration = SerializedConfiguration
            };
        }
    }
}
