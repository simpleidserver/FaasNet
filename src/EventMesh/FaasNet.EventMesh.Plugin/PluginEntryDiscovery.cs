using FaasNet.Common;
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
        public static bool TryExtract(string pluginDirectoryPath, IEnumerable<string> activePlugins, IEnumerable<Type> additionalSharedTypes, out IDiscoveredPlugin discoveryPlugin)
        {
            discoveryPlugin = null;
            var sharedTypes = new List<Type> { typeof(IPlugin<>), typeof(IServiceCollection) };
            if(additionalSharedTypes != null)
            {
                foreach(var additionalSharedType in additionalSharedTypes) sharedTypes.Add(additionalSharedType);
            }

            var pluginEntry = PluginConfigurationFile.Read(pluginDirectoryPath);
            if(pluginEntry == null) return false;
            if (!activePlugins.Contains(pluginEntry.Name)) return false;
            var dllPath = Path.Combine(pluginDirectoryPath, pluginEntry.DllName);
            if (!File.Exists(dllPath)) return false;
            var loader = PluginLoader.CreateFromAssemblyFile(
                dllPath,
                sharedTypes: sharedTypes.ToArray());
            var assembly = loader.LoadDefaultAssembly();
            var types = assembly.GetTypes();
            var pluginType = types.FirstOrDefault(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPlugin<>)));
            if (pluginType == null) return false;
            var serializedPluginConfiguration = string.Empty;
            discoveryPlugin = new DiscoveredPlugin(pluginEntry.Name, pluginType);
            return true;
        }

        private class DiscoveredPlugin : IDiscoveredPlugin
        {
            private readonly string _pluginName;
            private readonly Type _pluginType;

            public DiscoveredPlugin(string pluginName, Type pluginType)
            {
                _pluginName = pluginName;
                _pluginType = pluginType;
            }

            public void Load(ServerBuilder serverBuilder)
            {
                var pluginStore = serverBuilder.ServiceProvider.GetRequiredService<IPluginStore>();
                var optionType = _pluginType.GetInterfaces()[0].GenericTypeArguments[0];
                dynamic pluginConfiguration = Activator.CreateInstance(optionType);
                var pluginOption = pluginStore.GetOption(_pluginName);
                var serializedPluginConfiguration = string.Empty;
                if (pluginOption != null) serializedPluginConfiguration = JsonSerializer.Serialize(pluginOption);
                if (!string.IsNullOrWhiteSpace(serializedPluginConfiguration)) pluginConfiguration = JsonSerializer.Deserialize(serializedPluginConfiguration, optionType, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                var pluginInstance = Activator.CreateInstance(_pluginType);
                var loadFn = _pluginType.GetMethod("Load", BindingFlags.Public | BindingFlags.Instance);
                loadFn.Invoke(pluginInstance, new object[] { serverBuilder.Services, pluginConfiguration });
            }
        }
    }

    public interface IDiscoveredPlugin
    {
        void Load(ServerBuilder services);
    }
}
