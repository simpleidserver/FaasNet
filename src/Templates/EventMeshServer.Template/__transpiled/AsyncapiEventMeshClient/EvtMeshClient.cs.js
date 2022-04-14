'use strict';

require('source-map-support/register');
var _ = require('lodash');
var generatorReactSdk = require('@asyncapi/generator-react-sdk');
require('@asyncapi/parser');
var modelina = require('@asyncapi/modelina');
var jsxRuntime = require('react/cjs/react-jsx-runtime.production.min');

function _interopDefaultLegacy (e) { return e && typeof e === 'object' && 'default' in e ? e : { 'default': e }; }

var ___default = /*#__PURE__*/_interopDefaultLegacy(_);

function getChannelWrappers(channels) {
  const channelWrappers = [];

  for (const [channelName, channel] of channels) {
    Object.entries(channel.parameters());

    if (channel.hasSubscribe()) {
      const message = channel.subscribe().message(0);
      const messageType = modelina.FormatHelpers.toPascalCase(message.payload().uid());

      let msg = ___default["default"].camelCase(channelName);

      msg = msg.charAt(0).toUpperCase() + msg.slice(1);
      const record = `
			public Task<SubscriptionResult> Subscribe${msg}(Action<${messageType}> callback, CancellationToken cancellationToken = default(CancellationToken))
			{
				return SubscribeMessages("${channelName}", callback, cancellationToken);
			}
			`;
      channelWrappers.push(record);
    }

    if (channel.hasPublish()) {
      const message = channel.publish().message(0);
      const messageType = modelina.FormatHelpers.toPascalCase(message.payload().uid());
      const record = `
			public Task Publish(${messageType} parameter, CancellationToken cancellationToken = default(CancellationToken))
			{
				return Publish("${channelName}", parameter, cancellationToken);
			}
			`;
      channelWrappers.push(record);
    }
  }

  return channelWrappers;
}

function evtMeshClient({
  asyncapi
}) {
  const channelIterator = Object.entries(asyncapi.channels());
  const channels = getChannelWrappers(channelIterator);
  return /*#__PURE__*/jsxRuntime.jsx(generatorReactSdk.File, {
    name: 'EvtMeshClient.cs',
    children: `using FaasNet.EventMesh.Client;
using AsyncapiEventMeshClient.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace AsyncapiEventMeshClient
{
	public class EvtMeshClient : EventMeshClient
	{
		public EvtMeshClient(string clientId, string password, string vpn = Constants.DefaultVpn, string url = Constants.DefaultUrl, int port = Constants.DefaultPort, int bufferCloudEvents = 1) : base(clientId, password, vpn, url, port, bufferCloudEvents) { }
	
		${channels.join('\n')}
	}
}`
  });
}

module.exports = evtMeshClient;
//# sourceMappingURL=EvtMeshClient.cs.js.map
