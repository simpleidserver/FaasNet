using FaasNet.CLI.Configurations;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FaasNet.CLI.Helpers
{
    public static class ConfigurationHelper
    {
        public static string GatewayKey = "gateway";
        public static IEnumerable<string> AllConfigurationKeys = new string[]
        {
            GatewayKey
        };
        public static string ConfigurationFileName = "faasnet.yml";

        public static string GetConfigurationFilePath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), ConfigurationFileName);
        }

        public static FaasConfiguration GetConfiguration()
        {
            var path = GetConfigurationFilePath();
            if (!File.Exists(path))
            {
                return null;
            }

            var yml = File.ReadAllText(path);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
            return deserializer.Deserialize<FaasConfiguration>(yml);
        }

        public static void UpdateConfiguration(FaasConfiguration config)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
            var yml = serializer.Serialize(config);
            File.WriteAllText(GetConfigurationFilePath(), yml);
        }

        public static bool HasKey(string key)
        {
            return AllConfigurationKeys.Contains(key);
        }
    }
}
