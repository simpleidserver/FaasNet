using System.Collections.Generic;

namespace FaasNet.Runtime.Models
{
    public class Configuration
    {
        public Configuration(string name, string version)
        {
            ApiName = name;
            Version = version;
            Parameters = new List<ConfigurationParameter>();
        }

        public string ApiName { get; set; }
        public string Version { get; set; }
        public ICollection<ConfigurationParameter> Parameters { get; set; }
    }
}
