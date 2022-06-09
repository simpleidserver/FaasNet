using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public interface IPluginStore
    {
        IEnumerable<string> GetActivePlugins();
        void Enable(string pluginName);
        void Disable(string pluginName);
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

        public void Enable(string pluginName)
        {
            Update(pluginName, true);
        }

        public void Disable(string pluginName)
        {
            Update(pluginName, false);
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
            lock(_lock)
            {
                File.WriteAllText(GetPluginFilePath(), JsonSerializer.Serialize(record, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }));
            }
        }

        private ICollection<PluginRecord> Read()
        {
            lock(_lock)
            {
                var path = GetPluginFilePath();
                if (!File.Exists(path)) return new List<PluginRecord>();
                return JsonSerializer.Deserialize<ICollection<PluginRecord>>(File.ReadAllText(path), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }

        private static string GetPluginFilePath()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FILE_NAME);
            return path;
        }

        private class PluginRecord
        {
            public string Name { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
