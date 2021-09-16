namespace FaasNet.Runtime.Models
{
    public class ConfigurationTranslation
    {
        public ConfigurationTranslation(string language, string description)
        {
            Language = language;
            Description = description;
        }

        public string Language { get; set; }
        public string Description { get; set; }
    }
}
