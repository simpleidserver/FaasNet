using System.Collections.Generic;

namespace FaasNet.Runtime.Startup.Models
{
    public class ConfigurationParameter
    {
        public ConfigurationParameter(string name, string type)
        {
            Name = name;
            Type = type;
            Translations = new List<ConfigurationTranslation>();
        }

        public string Name { get; private set; }
        public string Type { get; private set; }
        public ICollection<ConfigurationTranslation> Translations { get; private set; }
    }
}
