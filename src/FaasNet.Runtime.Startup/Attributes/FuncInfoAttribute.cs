using System;

namespace FaasNet.Runtime.Startup.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FuncInfoAttribute : Attribute
    {
        public FuncInfoAttribute(string apiName, string version)
        {
            ApiName = apiName;
            Version = version;
        }

        public string ApiName { get; private set; }
        public string Version { get; private set; }
    }
}
