using McMaster.NETCore.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace FaasNet.EventMesh.Protocols
{
    public class ProtocolPluginEntryDiscovery
    {
        public static bool TryExtract(string pluginDirectoryPath, out IDiscoveredPlugin discoveryPlugin)
        {
            discoveryPlugin = null;
            var appsettingsFilePath = Path.Combine(pluginDirectoryPath, "appsettings.json");
            if (!File.Exists(appsettingsFilePath)) return false;
            var pluginEntry = JsonSerializer.Deserialize<ProtocolPluginEntry>(File.ReadAllText(appsettingsFilePath));
            var dllPath = Path.Combine(pluginDirectoryPath, pluginEntry.DllName);
            if(!File.Exists(dllPath)) return false;
            var loader = PluginLoader.CreateFromAssemblyFile(
                dllPath,
                sharedTypes: new[] { typeof(IProtocolPlugin<>), typeof(IServiceCollection) });
            var assembly = loader.LoadDefaultAssembly();
            var types = assembly.GetTypes();
            var pluginType = types.FirstOrDefault(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Name.StartsWith("IProtocolPlugin")));
            if (pluginType == null) return false;
            var optionType = pluginType.GetInterfaces()[0].GenericTypeArguments[0];
            dynamic option = Activator.CreateInstance(optionType);
            var serializedConfiguration = string.Empty;
            if(pluginEntry.Configuration != null) serializedConfiguration = JsonSerializer.Serialize(pluginEntry.Configuration);
            if(!string.IsNullOrWhiteSpace(serializedConfiguration)) option = JsonSerializer.Deserialize(serializedConfiguration, optionType, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            discoveryPlugin = new DiscoveredPlugin(pluginType, option);
            return true;
        }

        private class DiscoveredPlugin : IDiscoveredPlugin
        {
            private readonly Type _pluginType;
            private readonly dynamic _option;

            public DiscoveredPlugin(Type pluginType, dynamic option)
            {
                _pluginType = pluginType;
                _option = option;
            }

            public void Load(IServiceCollection services)
            {
                var pluginInstance = Activator.CreateInstance(_pluginType);
                var loadFn = _pluginType.GetMethod("Load", BindingFlags.Public | BindingFlags.Instance);
                loadFn.Invoke(pluginInstance, new object[] { services, _option });
            }
        }
    }

    public interface IDiscoveredPlugin
    {
        void Load(IServiceCollection services);
    }
}
