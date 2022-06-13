using McMaster.NETCore.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace FaasNet.EventMesh.Plugin
{
    public class PluginEntryDiscovery
    {
        public static bool TryExtract(string pluginDirectoryPath, IEnumerable<string> activePlugins, out IDiscoveredPlugin discoveryPlugin)
        {
            discoveryPlugin = null;
            var pluginEntry = PluginConfigurationFile.Read(pluginDirectoryPath);
            if(pluginEntry == null) return false;
            if (!activePlugins.Contains(pluginEntry.Name)) return false;
            var dllPath = Path.Combine(pluginDirectoryPath, pluginEntry.DllName);
            if (!File.Exists(dllPath)) return false;
            var loader = PluginLoader.CreateFromAssemblyFile(
                dllPath,
                sharedTypes: new[] { typeof(IPlugin<>), typeof(IServiceCollection) });
            var assembly = loader.LoadDefaultAssembly();
            var types = assembly.GetTypes();
            var pluginType = types.FirstOrDefault(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPlugin<>)));
            if (pluginType == null) return false;
            var optionType = pluginType.GetInterfaces()[0].GenericTypeArguments[0];
            var ttt = PluginEntryOption.Extract(optionType);
            dynamic pluginConfiguration = Activator.CreateInstance(optionType);
            var serializedPluginConfiguration = string.Empty;
            if (pluginEntry.Options != null) serializedPluginConfiguration = JsonSerializer.Serialize(pluginEntry.Options);
            if (!string.IsNullOrWhiteSpace(serializedPluginConfiguration)) pluginConfiguration = JsonSerializer.Deserialize(serializedPluginConfiguration, optionType, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            discoveryPlugin = new DiscoveredPlugin(pluginType, pluginConfiguration);
            return true;
        }

        private class DiscoveredPlugin : IDiscoveredPlugin
        {
            private readonly Type _pluginType;
            private readonly dynamic _pluginOption;

            public DiscoveredPlugin(Type pluginType, dynamic pluginOptions)
            {
                _pluginType = pluginType;
                _pluginOption = pluginOptions;
            }

            public void Load(IServiceCollection services)
            {
                var pluginInstance = Activator.CreateInstance(_pluginType);
                var loadFn = _pluginType.GetMethod("Load", BindingFlags.Public | BindingFlags.Instance);
                loadFn.Invoke(pluginInstance, new object[] { services, _pluginOption });
            }
        }
    }

    public interface IDiscoveredPlugin
    {
        void Load(IServiceCollection services);
    }
}
