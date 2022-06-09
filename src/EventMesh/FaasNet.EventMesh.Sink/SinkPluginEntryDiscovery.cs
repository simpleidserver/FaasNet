using FaasNet.EventMesh.Plugin;
using McMaster.NETCore.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace FaasNet.EventMesh.Sink
{
    public class SinkPluginEntryDiscovery
    {
        public static bool TryExtract(string pluginDirectoryPath, IEnumerable<string> activePlugins, out IDiscoveredPlugin discoveryPlugin)
        {
            discoveryPlugin = null;
            var appsettingsFilePath = Path.Combine(pluginDirectoryPath, PluginConstants.ConfigurationFileName);
            if (!File.Exists(appsettingsFilePath)) return false;
            var pluginEntry = JsonSerializer.Deserialize<SinkPluginEntry>(File.ReadAllText(appsettingsFilePath));
            if (!activePlugins.Contains(pluginEntry.Name)) return false;
            var dllPath = Path.Combine(pluginDirectoryPath, pluginEntry.DllName);
            if (!File.Exists(dllPath)) return false;
            var loader = PluginLoader.CreateFromAssemblyFile(
                dllPath,
                sharedTypes: new[] { typeof(ISinkPlugin<>), typeof(IServiceCollection) });
            var assembly = loader.LoadDefaultAssembly();
            var types = assembly.GetTypes();
            var pluginType = types.FirstOrDefault(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Name.StartsWith("ISinkPlugin")));
            if (pluginType == null) return false;
            var optionType = pluginType.GetInterfaces()[0].GenericTypeArguments[0];
            var sinkConfiguration = new SinkOptions();
            dynamic pluginConfiguration = Activator.CreateInstance(optionType);
            var serializedSinkConfiguration = string.Empty;
            var serializedPluginConfiguration = string.Empty;
            if (pluginEntry.SinkOptions != null) serializedSinkConfiguration = JsonSerializer.Serialize(pluginEntry.SinkOptions);
            if (pluginEntry.PluginOptions != null) serializedPluginConfiguration = JsonSerializer.Serialize(pluginEntry.PluginOptions);
            if (!string.IsNullOrWhiteSpace(serializedSinkConfiguration)) sinkConfiguration = JsonSerializer.Deserialize(serializedSinkConfiguration, typeof(SinkOptions), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) as SinkOptions;
            if (!string.IsNullOrWhiteSpace(serializedPluginConfiguration)) pluginConfiguration = JsonSerializer.Deserialize(serializedPluginConfiguration, optionType, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            discoveryPlugin = new DiscoveredPlugin(pluginType, sinkConfiguration, pluginConfiguration);
            return true;
        }

        private class DiscoveredPlugin : IDiscoveredPlugin
        {
            private readonly Type _pluginType;
            private readonly SinkOptions _sinkOptions;
            private readonly dynamic _pluginOption;

            public DiscoveredPlugin(Type pluginType, SinkOptions sinkOptions, dynamic pluginOptions)
            {
                _pluginType = pluginType;
                _sinkOptions = sinkOptions;
                _pluginOption = pluginOptions;
            }

            public void Load(IServiceCollection services)
            {
                var pluginInstance = Activator.CreateInstance(_pluginType);
                var loadFn = _pluginType.GetMethod("Load", BindingFlags.Public | BindingFlags.Instance);
                loadFn.Invoke(pluginInstance, new object[] { services, _sinkOptions, _pluginOption });
            }
        }
    }

    public interface IDiscoveredPlugin
    {
        void Load(IServiceCollection services);
    }
}
