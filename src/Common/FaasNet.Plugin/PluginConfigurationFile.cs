using System.IO;
using System.Text.Json;

namespace FaasNet.Plugin
{
    public class PluginConfigurationFile
    {
        private static object _lock = new object();

        public static PluginEntry Read(string directory)
        {
            lock(_lock)
            {
                var appsettingsFilePath = Path.Combine(directory, PluginConstants.ConfigurationFileName);
                if (!File.Exists(appsettingsFilePath)) return null;
                var pluginEntry = JsonSerializer.Deserialize<PluginEntry>(File.ReadAllText(appsettingsFilePath), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return pluginEntry;
            }
        }
    }
}
