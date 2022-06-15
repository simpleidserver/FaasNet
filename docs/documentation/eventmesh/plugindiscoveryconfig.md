# Configuration file Peer Discovery

**Name**

DiscoveryConfigFile

**Description**

Use configuration file to discover peers.

**Link**

The ZIP file can be downloaded [here]().

**Options**

| Name              | Description                           	| Default value 		                      |
| ----------------- | ----------------------------------------- | ------------------------------------------- |
| clusterNodes 		| Cluster nodes              				| [{\"Url\":\"localhost\",\"Port\":4000}]     |

## Quick start

Once you have an up and running EventMesh server with `DiscoveryConfigFile` plugin enabled, you can use the CLI to update the list of active peers for one specific EventMesh server.

```
FaasNet.EventMeshCTL.CLI.exe update_plugin_configuration --name=DiscoveryConfigFile --key=clusterNodes --value="[{\"Url\":\"localhost\",\"Port\":4000}]"
```

Enable the plugin and restart the EventMesh server to take into account your changes.

```
FaasNet.EventMeshCTL.CLI.exe enable_plugin --name=DiscoveryConfigFile
```