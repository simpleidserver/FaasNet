using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace FaasNet.Plugin
{
    public interface IPluginStore
    {
        IEnumerable<string> GetActivePlugins();
        object GetOption(string pluginName);
        void Enable(string pluginName);
        void Disable(string pluginName);
        bool UpdateOptions(string pluginName, object options);
        bool IsActive(string pluginName);
    }

    public class PluginStore : IPluginStore
    {
        private const string FILE_NAME = "plugin.json";
        private static object _lock = new object();

        public IEnumerable<string> GetActivePlugins()
        {
            var plugins = Read();
            return plugins.Where(p => p.IsActive).Select(p => p.Name);
        }

        public object GetOption(string pluginName)
        {
            var plugins = Read();
            return plugins.FirstOrDefault(p => p.Name == pluginName)?.Options;
        }

        public void Enable(string pluginName)
        {
            Update(pluginName, true);
        }

        public void Disable(string pluginName)
        {
            Update(pluginName, false);
        }

        public bool UpdateOptions(string pluginName, object options)
        {
            var records = Read();
            var record = records.FirstOrDefault(r => r.Name == pluginName);
            if (record == null)
            {
                records.Add(new PluginRecord
                {
                    Name = pluginName,
                    IsActive = false,
                    Options = options
                });
            }
            else record.Options = options;
            lock (_lock)
            {
                File.WriteAllText(GetPluginFilePath(), JsonSerializer.Serialize(records, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }));
            }

            return true;
        }

        public bool IsActive(string pluginName)
        {
            var pluginRecords = Read();
            return pluginRecords.Any(p => p.Name == pluginName && p.IsActive);
        }

        private void Update(string pluginName, bool isEnabled)
        {
            var records = Read();
            var record = records.FirstOrDefault(r => r.Name == pluginName);
            if (record == null)
            {
                records.Add(new PluginRecord
                {
                    Name = pluginName,
                    IsActive = isEnabled
                });
            }
            else record.IsActive = isEnabled;
            lock (_lock)
            {
                File.WriteAllText(GetPluginFilePath(), JsonSerializer.Serialize(records, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }));
            }
        }

        private ICollection<PluginRecord> Read()
        {
            lock (_lock)
            {
                var path = GetPluginFilePath();
                if (!File.Exists(path)) return new List<PluginRecord>();
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<List<PluginRecord>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }

        private static string GetPluginFilePath()
        {
            var env = Environment.GetEnvironmentVariable("EVENTMESH_PLUGIN");
            if (!string.IsNullOrWhiteSpace(env)) return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"plugin.{env}.json");
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FILE_NAME);
            return path;
        }

        private class PluginRecord
        {
            public string Name { get; set; }
            public bool IsActive { get; set; }
            public object Options { get; set; }
        }
    }
}
