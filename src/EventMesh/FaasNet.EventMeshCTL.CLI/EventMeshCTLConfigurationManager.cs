using System.Text.Json;

namespace FaasNet.EventMeshCTL.CLI
{
    public class EventMeshCTLConfigurationManager
    {
        private static EventMeshCTLConfiguration _cachedConfiguration;
        private static object _lock = new object();
        private const string FILE_NAME = "appsettings.json";

        public static EventMeshCTLConfiguration Get()
        {
            lock(_lock)
            {
                if (_cachedConfiguration != null) return _cachedConfiguration;
                var path = GetConfigurationFilePath();
                var json = File.ReadAllText(path);
                var result = JsonSerializer.Deserialize<EventMeshCTLConfiguration>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (result == null) throw new InvalidOperationException($"The configuration file {FILE_NAME} is not correct");
                return result;
            }
        }

        public void Update(EventMeshCTLConfiguration configuration)
        {
            lock(_lock)
            {
                _cachedConfiguration = configuration;
                var path = GetConfigurationFilePath();
                var json = JsonSerializer.Serialize(configuration, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                File.WriteAllText(path, json);
            }
        }

        private static string GetConfigurationFilePath()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FILE_NAME);
            if (!File.Exists(path)) throw new InvalidOperationException($"The configuration file {FILE_NAME} doesn't exist");
            return path;
        }
    }

    public class EventMeshCTLConfiguration
    {
        public int Port { get; set; } = EventMesh.Client.Constants.DefaultPort;
        public string Url { get; set; } = EventMesh.Client.Constants.DefaultUrl;
    }
}
