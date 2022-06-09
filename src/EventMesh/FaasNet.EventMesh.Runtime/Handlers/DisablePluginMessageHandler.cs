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
    public class DisablePluginMessageHandler : IMessageHandler
    {
        private readonly EventMeshNodeOptions _options;
        private readonly IPluginStore _pluginStore;

        public DisablePluginMessageHandler(IOptions<EventMeshNodeOptions> options, IPluginStore pluginStore)
        {
            _options = options.Value;
            _pluginStore = pluginStore;
        }

        public Commands Command => Commands.DISABLE_PLUGIN_REQUEST;

        public Task<EventMeshPackageResult> Run(Package package, IEnumerable<IPeerHost> peers, CancellationToken cancellationToken)
        {
            var disablePluginRequest = package as DisablePluginRequest;
            var allPlugins = GetAllPluginsMessageHandler.ExtractPluginInfos(_options.ProtocolsPluginSubPath);
            allPlugins.AddRange(GetAllPluginsMessageHandler.ExtractPluginInfos(_options.SinksPluginSubPath));
            var selectedPlugin = allPlugins.FirstOrDefault(p => p.Name == disablePluginRequest.PluginName);
            if (selectedPlugin == null) return Task.FromResult(EventMeshPackageResult.SendResult(PackageResponseBuilder.Error(disablePluginRequest.Header.Command, disablePluginRequest.Header.Seq, Errors.UNKNOWN_PLUGIN)));
            _pluginStore.Disable(disablePluginRequest.PluginName);
            return Task.FromResult(EventMeshPackageResult.SendResult(PackageResponseBuilder.DisablePlugin(disablePluginRequest.Header.Seq)));
        }
    }
}
