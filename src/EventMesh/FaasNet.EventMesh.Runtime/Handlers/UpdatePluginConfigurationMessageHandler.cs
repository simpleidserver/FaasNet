using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Plugin;
using FaasNet.RaftConsensus.Core;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class UpdatePluginConfigurationMessageHandler : IMessageHandler
    {
        private readonly EventMeshNodeOptions _options;

        public UpdatePluginConfigurationMessageHandler(IOptions<EventMeshNodeOptions> options)
        {
            _options = options.Value;
        }

        public Commands Command => Commands.UPDATE_PLUGIN_CONFIGURATION_REQUEST;

        public Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            var updatePluginConfiguration = package as UpdatePluginConfigurationRequest;
            var result = UpdateConfiguration(_options.ProtocolsPluginSubPath, updatePluginConfiguration.Name, updatePluginConfiguration.PropertyName, updatePluginConfiguration.PropertyValue);
            if (result == UpdateConfigurationResult.UNKNOWNPLUGIN) result = UpdateConfiguration(_options.SinksPluginSubPath, updatePluginConfiguration.Name, updatePluginConfiguration.PropertyName, updatePluginConfiguration.PropertyValue);
            if (result == UpdateConfigurationResult.UNKNOWNPROPERTY) Task.FromResult(EventMeshPackageResult.SendResult(PackageResponseBuilder.Error(package.Header.Command, package.Header.Seq, Errors.UNKNOWN_PLUGIN_PROPERTY)));
            if(result == UpdateConfigurationResult.SUCCESS) return Task.FromResult(EventMeshPackageResult.SendResult(PackageResponseBuilder.UpdatePluginConfiguration(package.Header.Seq)));
            return Task.FromResult(EventMeshPackageResult.SendResult(PackageResponseBuilder.Error(package.Header.Command, package.Header.Seq, Errors.UNKNOWN_PLUGIN)));
        }

        public UpdateConfigurationResult UpdateConfiguration(string subPath, string name, string propertyKey, string propertyValue)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var rootPluginPath = Path.Combine(baseDirectory, subPath);
            var allPluginPath = Directory.EnumerateDirectories(rootPluginPath);
            foreach (var pluginPath in allPluginPath)
            {
                var pluginEntry = PluginConfigurationFile.Read(pluginPath);
                if (pluginEntry.Name != name) continue;
                var dllPath = Path.Combine(pluginPath, pluginEntry.DllName);
                var assembly = Assembly.LoadFrom(dllPath);
                var types = assembly.GetTypes();
                var pluginType = types.FirstOrDefault(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPlugin<>)));
                var optionType = pluginType.GetInterfaces()[0].GenericTypeArguments[0];
                var entries = PluginEntryOption.Extract(optionType);
                var selectedProperty = entries.FirstOrDefault(e => e.Name == propertyKey);
                if (selectedProperty == null) return UpdateConfigurationResult.UNKNOWNPROPERTY;
                var serializedPluginConfiguration = string.Empty;
                dynamic pluginConfiguration = Activator.CreateInstance(optionType);
                if (pluginEntry.Options != null) serializedPluginConfiguration = JsonSerializer.Serialize(pluginEntry.Options); 
                if (!string.IsNullOrWhiteSpace(serializedPluginConfiguration)) pluginConfiguration = JsonSerializer.Deserialize(serializedPluginConfiguration, optionType, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                SetValue(pluginConfiguration, selectedProperty.PropertyInfo, propertyValue);
                pluginEntry.Options = pluginConfiguration;
                PluginConfigurationFile.Write(pluginPath, pluginEntry);
                return UpdateConfigurationResult.SUCCESS;
            }

            return UpdateConfigurationResult.UNKNOWNPLUGIN;
        }

        public enum UpdateConfigurationResult
        {
            UNKNOWNPLUGIN = 0,
            UNKNOWNPROPERTY = 1,
            SUCCESS = 2
        }

        private static void SetValue(object obj, PropertyInfo propertyInfo, string value)
        {
            object result = value;
            if (propertyInfo.PropertyType == typeof(int)) result = int.Parse(value);
            propertyInfo.SetValue(obj, result);
        }
    }
}
