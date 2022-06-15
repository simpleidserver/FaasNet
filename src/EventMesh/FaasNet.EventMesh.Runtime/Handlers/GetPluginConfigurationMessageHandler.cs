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
    public class GetPluginConfigurationMessageHandler : IMessageHandler
    {
        private readonly EventMeshNodeOptions _options;

        public GetPluginConfigurationMessageHandler(IOptions<EventMeshNodeOptions> options)
        {
            _options = options.Value;
        }

        public Commands Command => Commands.GET_PLUGIN_CONFIGURATION_REQUEST;

        public Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            List<PluginConfigurationRecordResponse> result;
            var getPluginConfiguration = package as GetPluginConfigurationRequest;
            if (TryExtractConfiguration(_options.ProtocolsPluginSubPath, getPluginConfiguration.Name, out result)) return Task.FromResult(EventMeshPackageResult.SendResult(PackageResponseBuilder.GetPluginConfiguration(result, package.Header.Seq)));
            if (TryExtractConfiguration(_options.SinksPluginSubPath, getPluginConfiguration.Name, out result)) return Task.FromResult(EventMeshPackageResult.SendResult(PackageResponseBuilder.GetPluginConfiguration(result, package.Header.Seq)));
            if (TryExtractConfiguration(_options.DiscoveriesPluginSubPath, getPluginConfiguration.Name, out result)) return Task.FromResult(EventMeshPackageResult.SendResult(PackageResponseBuilder.GetPluginConfiguration(result, package.Header.Seq)));
            return Task.FromResult(EventMeshPackageResult.SendResult(PackageResponseBuilder.Error(package.Header.Command, package.Header.Seq, Errors.UNKNOWN_PLUGIN)));
        }

        public bool TryExtractConfiguration(string subPath, string name, out List<PluginConfigurationRecordResponse> result)
        {
            result = null;
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
                dynamic pluginConfiguration = Activator.CreateInstance(optionType);
                var entries = PluginEntryOption.Extract(optionType);
                var serializedPluginConfiguration = string.Empty;
                if (pluginEntry.Options != null) serializedPluginConfiguration = JsonSerializer.Serialize(pluginEntry.Options);
                if (!string.IsNullOrWhiteSpace(serializedPluginConfiguration)) pluginConfiguration = JsonSerializer.Deserialize(serializedPluginConfiguration, optionType, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                result = new List<PluginConfigurationRecordResponse>();
                foreach (var entry in entries)
                {
                    var value = JsonSerializer.Serialize(entry.PropertyInfo.GetValue(pluginConfiguration));
                    result.Add(new PluginConfigurationRecordResponse(entry.Name, entry.Description, entry.DefaultValue, value));
                }

                return true;
            }

            return false;
        }
    }
}
