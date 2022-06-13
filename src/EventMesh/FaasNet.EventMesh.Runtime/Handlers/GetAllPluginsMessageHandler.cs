using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Plugin;
using FaasNet.EventMesh.Runtime.Stores;
using FaasNet.RaftConsensus.Core;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class GetAllPluginsMessageHandler : IMessageHandler
    {
        private readonly EventMeshNodeOptions _options;
        private readonly IPluginStore _pluginStore;

        public GetAllPluginsMessageHandler(IOptions<EventMeshNodeOptions> options, IPluginStore pluginStore)
        {
            _options = options.Value;
            _pluginStore = pluginStore;
        }

        public Commands Command => Commands.GET_ALL_PLUGINS_REQUEST;

        public Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            var plugins = ExtractPlugins(_options.ProtocolsPluginSubPath);
            plugins.AddRange(ExtractPlugins(_options.SinksPluginSubPath));
            return Task.FromResult(EventMeshPackageResult.SendResult(PackageResponseBuilder.GetAllPlugins(plugins, package.Header.Seq)));
        }

        private List<PluginResponse> ExtractPlugins(string subPath)
        {
            var pluginInfos = ExtractPluginInfos(subPath);
            foreach(var pluginInfo in pluginInfos) pluginInfo.IsActive = _pluginStore.IsActive(pluginInfo.Name);
            return pluginInfos;
        }

        public static List<PluginResponse> ExtractPluginInfos(string subPath)
        {
            var result = new List<PluginResponse>();
            var rootPluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, subPath);
            var allPluginPath = Directory.EnumerateDirectories(rootPluginPath);
            foreach (var pluginPath in allPluginPath)
            {
                var pluginEntry = PluginConfigurationFile.Read(pluginPath);
                result.Add(new PluginResponse
                {
                    Name = pluginEntry.Name,
                    Description = pluginEntry.Description
                });
            }

            return result;
        }
    }
}
