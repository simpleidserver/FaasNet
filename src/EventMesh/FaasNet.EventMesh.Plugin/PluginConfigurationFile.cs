using System.IO;
using System.Text.Json;

namespace FaasNet.EventMesh.Plugin
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

        public static void Write(string directory, PluginEntry entry)
        {
            lock (_lock)
            {
                var appsettingsFilePath = Path.Combine(directory, PluginConstants.ConfigurationFileName);
                if (!File.Exists(appsettingsFilePath)) return;
                var json = JsonSerializer.Serialize<PluginEntry>(entry, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                File.WriteAllText(appsettingsFilePath, json);
            }
        }
    }
}
