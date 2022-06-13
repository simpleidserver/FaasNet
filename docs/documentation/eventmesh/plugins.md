# Plugins

EventMesh server supports plugins. Plugins extend core functionality in a variety of ways: 
* Support more protocols.
* Support more sink connectors. A sink connector consumes records from an event store or message broker and stream the data events to the EventMesh server.

## Install a plugin

A plugin can manually be installed :
1. Download the zip file.
2. Extract its content.
3. Based on the nature of the plugin, move the folder into the appropriate sub-folder of the local EventMesh server.
   * If the plugin supports an additional protocol then the sub-folder is `protocolPlugins`.
   * If the plugin supports a new sink connector then the sub-folder is `sinkPlugins`.

## Enable a plugin

Once the plugin is installed, open a command prompt and execute the command `FaasNet.EventMeshCTL.CLI.exe  enable_plugin --name=<PLUGIN_NAME>` to enable the plugin.

If the plugin is successfully enabled then a success message is displayed.

## Configure a plugin

There are two methods to configure a plugin. Either by editing the `appsettings.json` configuration file present inside the plugin directory or by using the CLI.

All the available configuration records can be displayed by executing the command line 

```
FaasNet.EventMeshCTL.CLI.exe get_plugin_configuration --name=<PLUGIN_NAME>
```

For each configuration record, the CLI displays :
* Unique property name.
* Short description.
* Configured value (coming from the configuration file).

Any configuration record can be updated like this 

```
FaasNet.EventMeshCTL.CLI.exe update_plugin_configuration --name=<PLUGIN_NAME> --key=<PROPERTY_KEY> --value=<PROPERTY_VALUE>
```

> [!IMPORTANT]
> Once the plugin is properly configured. The EventMesh server must be restarted in order to take into account your modifications.

## Supported plugins

The table below lists plugins

| Plugin name                                 | Folder           | Description                                         |
| ------------------------------------------- | ---------------- | --------------------------------------------------- |
| [ProtocolAmqp](pluginamqp.md)               | protocolPlugins  | Support AMQP1.0 protocol                            |
| [ProtocolWebsocket](pluginwebsocket.md)     | protocolPlugins  | Support WebSocket protocol                          |
| [SinkAMQP](pluginsinkamqp.md)               | sinkPlugins      | Consume records from AMQP server                    |
| [SinkKafka](pluginsinkkafka.md)             | sinkPlugins      | Consume records from KAFKA server                   |
| [SinkVpnBridge](pluginsinkvpnbridge.md)     | sinkPlugins      | Consume records from Virtual Private Network bridge |