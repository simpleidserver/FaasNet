using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Runtime.Stores;
using FaasNet.RaftConsensus.Core;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class EnablePluginMessageHandler : IMessageHandler
    {
        private readonly EventMeshNodeOptions _options;
        private readonly IPluginStore _pluginStore;

        public EnablePluginMessageHandler(IOptions<EventMeshNodeOptions> options, IPluginStore pluginStore)
        {
            _options = options.Value;
            _pluginStore = pluginStore;
        }

        public Commands Command => Commands.ENABLE_PLUGIN_REQUEST;

        public Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            var enablePluginRequest = package as EnablePluginRequest;
            var allPlugins = GetAllPluginsMessageHandler.ExtractPluginInfos(_options.ProtocolsPluginSubPath);
            allPlugins.AddRange(GetAllPluginsMessageHandler.ExtractPluginInfos(_options.SinksPluginSubPath));
            var allDiscoveryPlugin = GetAllPluginsMessageHandler.ExtractPluginInfos(_options.DiscoveriesPluginSubPath);
            allPlugins.AddRange(allDiscoveryPlugin);
            var selectedPlugin = allPlugins.FirstOrDefault(p => p.Name == enablePluginRequest.PluginName);
            if (selectedPlugin == null) return Task.FromResult(EventMeshPackageResult.SendResult(PackageResponseBuilder.Error(enablePluginRequest.Header.Command, enablePluginRequest.Header.Seq, Errors.UNKNOWN_PLUGIN)));
            DisableUnusedDiscoveryPlugins(allDiscoveryPlugin, enablePluginRequest.PluginName);
            _pluginStore.Enable(enablePluginRequest.PluginName);
            return Task.FromResult(EventMeshPackageResult.SendResult(PackageResponseBuilder.EnablePlugin(enablePluginRequest.Header.Seq)));
        }

        private void DisableUnusedDiscoveryPlugins(List<PluginResponse> allDiscoveryPlugin, string pluginName)
        {
            var isDiscoveryPlugin = allDiscoveryPlugin.Any(p => p.Name == pluginName);
            if (!isDiscoveryPlugin) return;
            var unusedDiscoveryPlugin = allDiscoveryPlugin.Where(p => p.Name != pluginName);
            foreach (var plugin in unusedDiscoveryPlugin) _pluginStore.Disable(plugin.Name);
        }
    }
}
