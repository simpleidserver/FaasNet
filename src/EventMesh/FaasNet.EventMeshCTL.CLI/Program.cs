using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Exceptions;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.RaftConsensus.Client;
using McMaster.Extensions.CommandLineUtils;

namespace FaasNet.EventMeshCTL.CLI
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.HelpOption(inherited: true);
            AddVersionCommand(app);
            AddClusterStatusCommand(app);
            AddVpnCommand(app);
            AddBridgeCommand(app);
            AddClientCommand(app);
            AddGetAllVpnCommand(app);
            AddGetPluginsCommand(app);
            AddEnablePluginCommand(app);
            AddDisablePluginCommand(app);
            AddGetPluginConfiguration(app);
            AddUpdatePluginConfiguration(app);
            AddPublishMessageConfiguration(app);
            AddReadMessageConfiguration(app);
            app.OnExecute(() =>
            {
                Console.WriteLine("Specify a command");
                app.ShowHelp();
                return 1;
            });
            return app.Execute(args);
        }

        private static void AddVersionCommand(CommandLineApplication app)
        {
            app.Command("version", versionCmd =>
            {
                versionCmd.Description = "Current version";
                versionCmd.OnExecute(() =>
                {
                    var version = typeof(Program).Assembly.GetName().Version;
                    Console.WriteLine($"The current version is {version}");
                    return 1;
                });
            });
        }

        private static void AddClusterStatusCommand(CommandLineApplication app)
        {
            app.Command("cluster_status", clusterStatusCmd =>
            {
                clusterStatusCmd.Description = "Current nodes in the cluster";
                var urlOptions = clusterStatusCmd.Option<string>("-u|--url <URL>", "EventMesh URL", CommandOptionType.SingleValue);
                urlOptions.DefaultValue = Constants.DefaultUrl;
                var portOptions = clusterStatusCmd.Option<int>("-p|--port <PORT>", "EventMesh Port", CommandOptionType.SingleValue);
                portOptions.DefaultValue = Constants.DefaultPort;
                clusterStatusCmd.OnExecuteAsync(async (token) =>
                {
                    var gossipClient = new GossipClient(urlOptions.ParsedValue, portOptions.ParsedValue);
                    var clusterNodes = await gossipClient.GetClusterNodes();
                    Console.WriteLine("Current nodes");
                    foreach (var clusterNode in clusterNodes) Console.WriteLine($"Url={clusterNode.Url}, Port={clusterNode.Port}");
                    return 1;
                });
            });
        }

        private static void AddVpnCommand(CommandLineApplication app)
        {
            app.Command("add_vpn", addVpnCmd =>
            {
                var nameOption = addVpnCmd.Option<string>("-n|--name <NAME>", "The name", CommandOptionType.SingleValue);
                nameOption.DefaultValue = Constants.DefaultVpn;
                var urlOptions = addVpnCmd.Option<string>("-u|--url <URL>", "EventMesh URL", CommandOptionType.SingleValue);
                urlOptions.DefaultValue = Constants.DefaultUrl;
                var portOptions = addVpnCmd.Option<int>("-p|--port <PORT>", "EventMesh Port", CommandOptionType.SingleValue);
                portOptions.DefaultValue = Constants.DefaultPort;
                addVpnCmd.Description = "Add VPN";
                addVpnCmd.OnExecuteAsync(async (token) =>
                {
                    var vpnName = nameOption.ParsedValue;
                    var evtMeshClient = new EventMeshClient(urlOptions.ParsedValue, portOptions.ParsedValue);
                    try
                    {
                        await evtMeshClient.AddVpn(vpnName, token);
                    }
                    catch(RuntimeClientResponseException ex)
                    {
                        DisplayError(ex);
                        return;
                    }

                    DisplaySuccess($"VPN {vpnName} has been added");
                });
            });
        }

        private static void AddBridgeCommand(CommandLineApplication app)
        {
            app.Command("add_vpn_bridge", addVpnBridgeCmd =>
            {
                addVpnBridgeCmd.Description = "Add bridge between two Message VPN";
                var sourceVpnOption = addVpnBridgeCmd.Option<string>("-v|--vpn <VPN>", "Source VPN", CommandOptionType.SingleValue);
                sourceVpnOption.DefaultValue = Constants.DefaultVpn;
                var targetVpnOptions = addVpnBridgeCmd.Option<string>("-tv|--tvpn <VPN>", "Target VPN", CommandOptionType.SingleValue);
                targetVpnOptions.DefaultValue = Constants.DefaultVpn;
                var targetUrnOptions = addVpnBridgeCmd.Option<string>("-tu|--turn <URN>", "Target urn", CommandOptionType.SingleValue);
                targetUrnOptions.IsRequired();
                var targetPortOptions = addVpnBridgeCmd.Option<int>("-tp|--tport <URN>", "Target port", CommandOptionType.SingleValue);
                targetPortOptions.DefaultValue = Constants.DefaultPort;
                var targetClientIdOptions = addVpnBridgeCmd.Option<string>("-ti|--tid <CLIENT_ID>", "Target client identifier", CommandOptionType.SingleValue);
                targetClientIdOptions.IsRequired();
                var urlOptions = addVpnBridgeCmd.Option<string>("-u|--url <URL>", "EventMesh URL", CommandOptionType.SingleValue);
                urlOptions.DefaultValue = Constants.DefaultUrl;
                var portOptions = addVpnBridgeCmd.Option<int>("-p|--port <PORT>", "EventMesh Port", CommandOptionType.SingleValue);
                portOptions.DefaultValue = Constants.DefaultPort;
                addVpnBridgeCmd.Description = "Add VPN";
                addVpnBridgeCmd.OnExecuteAsync(async (token) =>
                {
                    var vpnName = sourceVpnOption.ParsedValue;
                    var evtMeshClient = new EventMeshClient(urlOptions.ParsedValue, portOptions.ParsedValue);
                    try
                    {
                        await evtMeshClient.AddBridge(sourceVpnOption.ParsedValue, targetUrnOptions.ParsedValue, targetPortOptions.ParsedValue, targetVpnOptions.ParsedValue, targetClientIdOptions.ParsedValue, token);
                    }
                    catch (RuntimeClientResponseException ex)
                    {
                        DisplayError(ex);
                        return;
                    }

                    DisplaySuccess("VPN Bridge has been added");
                });
            });
        }

        private static void AddGetAllVpnCommand(CommandLineApplication app)
        {
            app.Command("get_all_vpn", getAllVpnCmd =>
            {
                getAllVpnCmd.Description = "Get all VPN";
                var urlOptions = getAllVpnCmd.Option<string>("-u|--url <URL>", "EventMesh URL", CommandOptionType.SingleValue);
                urlOptions.DefaultValue = Constants.DefaultUrl;
                var portOptions = getAllVpnCmd.Option<int>("-p|--port <PORT>", "EventMesh Port", CommandOptionType.SingleValue);
                portOptions.DefaultValue = Constants.DefaultPort;
                getAllVpnCmd.OnExecuteAsync(async (token) =>
                {
                    var evtMeshClient = new EventMeshClient(urlOptions.ParsedValue, portOptions.ParsedValue);
                    var allVpns = await evtMeshClient.GetAllVpns(token);
                    foreach (var vpn in allVpns) Console.WriteLine(vpn);
                });
            });
        }

        private static void AddClientCommand(CommandLineApplication app)
        {
            app.Command("add_client", addClientCmd =>
            {
                addClientCmd.Description = "Add client";
                var vpnOption = addClientCmd.Option<string>("-v|--vpn <VPN>", "The VPN", CommandOptionType.SingleValue);
                vpnOption.DefaultValue = Constants.DefaultVpn;
                var idOption = addClientCmd.Option<string>("-id|--identifier <LOGIN>", "The client identifier", CommandOptionType.SingleValue);
                idOption.DefaultValue = "clientId";
                var pubEnabled = addClientCmd.Option<bool>("-p|--publish_enabled <PUBLISH>", "Enable publish", CommandOptionType.SingleValue);
                pubEnabled.DefaultValue = true;
                var subEnabled = addClientCmd.Option<bool>("-s|--subscription_enabled <SUBSCRIPTION>", "Enable subscription", CommandOptionType.SingleValue);
                subEnabled.DefaultValue = true;
                var urlOptions = addClientCmd.Option<string>("-u|--url <URL>", "EventMesh URL", CommandOptionType.SingleValue);
                urlOptions.DefaultValue = Constants.DefaultUrl;
                var portOptions = addClientCmd.Option<int>("-p|--port <PORT>", "EventMesh Port", CommandOptionType.SingleValue);
                portOptions.DefaultValue = Constants.DefaultPort;
                addClientCmd.OnExecuteAsync(async (token) =>
                {
                    var evtMeshClient = new EventMeshClient(urlOptions.ParsedValue, portOptions.ParsedValue);
                    var purposes = new List<UserAgentPurpose>();
                    if (pubEnabled.ParsedValue) purposes.Add(UserAgentPurpose.PUB);
                    if (subEnabled.ParsedValue) purposes.Add(UserAgentPurpose.SUB);
                    try
                    {
                        await evtMeshClient.AddClient(vpnOption.ParsedValue, idOption.ParsedValue, purposes, token);
                    }
                    catch (RuntimeClientResponseException ex)
                    {
                        DisplayError(ex);
                        return;
                    }

                    DisplaySuccess($"Client {idOption.ParsedValue} has been added");
                });
            });
        }

        private static void AddGetPluginsCommand(CommandLineApplication app)
        {
            app.Command("get_plugins", getPluginsCmd =>
            {
                getPluginsCmd.Description = "Get all plugins";
                var urlOptions = getPluginsCmd.Option<string>("-u|--url <URL>", "EventMesh URL", CommandOptionType.SingleValue);
                urlOptions.DefaultValue = Constants.DefaultUrl;
                var portOptions = getPluginsCmd.Option<int>("-p|--port <PORT>", "EventMesh Port", CommandOptionType.SingleValue);
                portOptions.DefaultValue = Constants.DefaultPort;
                getPluginsCmd.OnExecuteAsync(async (token) =>
                {
                    var evtMeshClient = new EventMeshClient(urlOptions.ParsedValue, portOptions.ParsedValue);
                    var plugins = await evtMeshClient.GetAllPlugins(token);
                    foreach (var plugin in plugins)
                    {
                        Console.WriteLine($"Name = {plugin.Name}");
                        Console.WriteLine($"Description = {plugin.Description}");
                        Console.WriteLine($"Is active = {plugin.IsActive}");
                        Console.WriteLine();
                    }
                });
            });
        }

        private static void AddEnablePluginCommand(CommandLineApplication app)
        {
            app.Command("enable_plugin", enablePluginCmd =>
            {
                enablePluginCmd.Description = "Enable plugin";
                var nameOption = enablePluginCmd.Option<string>("-n|--name <NAME>", "The name", CommandOptionType.SingleValue);
                nameOption.IsRequired();
                var urlOptions = enablePluginCmd.Option<string>("-u|--url <URL>", "EventMesh URL", CommandOptionType.SingleValue);
                urlOptions.DefaultValue = Constants.DefaultUrl;
                var portOptions = enablePluginCmd.Option<int>("-p|--port <PORT>", "EventMesh Port", CommandOptionType.SingleValue);
                portOptions.DefaultValue = Constants.DefaultPort;
                enablePluginCmd.OnExecuteAsync(async (token) =>
                {
                    var evtMeshClient = new EventMeshClient(urlOptions.ParsedValue, portOptions.ParsedValue);
                    await evtMeshClient.EnablePlugin(nameOption.ParsedValue, token);
                    DisplaySuccess("Plugin is enabled");
                });
            });
        }

        private static void AddDisablePluginCommand(CommandLineApplication app)
        {
            app.Command("disable_plugin", disablePluginCmd =>
            {
                disablePluginCmd.Description = "Disable plugin";
                var nameOption = disablePluginCmd.Option<string>("-n|--name <NAME>", "The name", CommandOptionType.SingleValue);
                nameOption.IsRequired();
                var urlOptions = disablePluginCmd.Option<string>("-u|--url <URL>", "EventMesh URL", CommandOptionType.SingleValue);
                urlOptions.DefaultValue = Constants.DefaultUrl;
                var portOptions = disablePluginCmd.Option<int>("-p|--port <PORT>", "EventMesh Port", CommandOptionType.SingleValue);
                portOptions.DefaultValue = Constants.DefaultPort;
                disablePluginCmd.OnExecuteAsync(async (token) =>
                {
                    var evtMeshClient = new EventMeshClient(urlOptions.ParsedValue, portOptions.ParsedValue);
                    await evtMeshClient.DisablePlugin(nameOption.ParsedValue, token);
                    DisplaySuccess("Plugin is disabled");
                });
            });
        }

        private static void AddGetPluginConfiguration(CommandLineApplication app)
        {
            app.Command("get_plugin_configuration", disablePluginCmd =>
            {
                disablePluginCmd.Description = "Get plugin configuration";
                var nameOption = disablePluginCmd.Option<string>("-n|--name <NAME>", "Plugin name", CommandOptionType.SingleValue);
                nameOption.IsRequired();
                var urlOptions = disablePluginCmd.Option<string>("-u|--url <URL>", "EventMesh URL", CommandOptionType.SingleValue);
                urlOptions.DefaultValue = Constants.DefaultUrl;
                var portOptions = disablePluginCmd.Option<int>("-p|--port <PORT>", "EventMesh Port", CommandOptionType.SingleValue);
                portOptions.DefaultValue = Constants.DefaultPort;
                disablePluginCmd.OnExecuteAsync(async (token) =>
                {
                    var evtMeshClient = new EventMeshClient(urlOptions.ParsedValue, portOptions.ParsedValue);
                    try
                    {
                        var configurationRecords = await evtMeshClient.GetPluginConfiguration(nameOption.ParsedValue, token);
                        foreach (var record in configurationRecords)
                        {
                            Console.WriteLine($"Name = {record.Name}");
                            Console.WriteLine($"Description = {record.Description}");
                            Console.WriteLine($"Configured value = {record.ConfiguredValue}");
                            Console.WriteLine();
                        }
                    }
                    catch(RuntimeClientResponseException ex)
                    {
                        DisplayError(ex);
                    }
                });
            });
        }

        private static void AddUpdatePluginConfiguration(CommandLineApplication app)
        {
            app.Command("update_plugin_configuration", updatePluginCmd =>
            {
                updatePluginCmd.Description = "Update plugin configuration";
                var nameOption = updatePluginCmd.Option<string>("-n|--name <NAME>", "Plugin name", CommandOptionType.SingleValue);
                nameOption.IsRequired();
                var propertyKey = updatePluginCmd.Option<string>("-k|--key <KEY>", "Property key", CommandOptionType.SingleValue);
                propertyKey.IsRequired();
                var propertyValue = updatePluginCmd.Option<string>("-v|--value <VALUE>", "Property value", CommandOptionType.SingleValue);
                propertyValue.IsRequired();
                var urlOptions = updatePluginCmd.Option<string>("-u|--url <URL>", "EventMesh URL", CommandOptionType.SingleValue);
                urlOptions.DefaultValue = Constants.DefaultUrl;
                var portOptions = updatePluginCmd.Option<int>("-p|--port <PORT>", "EventMesh Port", CommandOptionType.SingleValue);
                portOptions.DefaultValue = Constants.DefaultPort;
                updatePluginCmd.OnExecuteAsync(async (token) =>
                {
                    var evtMeshClient = new EventMeshClient(urlOptions.ParsedValue, portOptions.ParsedValue);
                    try
                    {
                        await evtMeshClient.UpdatePluginConfiguration(nameOption.ParsedValue, propertyKey.ParsedValue, propertyValue.ParsedValue, token);
                        DisplaySuccess("Plugin configuration has been updated");
                    }
                    catch (RuntimeClientResponseException ex)
                    {
                        DisplayError(ex);
                    }
                });
            });
        }

        private static void AddPublishMessageConfiguration(CommandLineApplication app)
        {
            app.Command("publish_message", publishMsgCmd =>
            {
                publishMsgCmd.Description = "Publish one message";
                var vpnOption = publishMsgCmd.Option<string>("-v|--vpn <VPN>", "The VPN", CommandOptionType.SingleValue);
                vpnOption.DefaultValue = Constants.DefaultVpn;
                var idOption = publishMsgCmd.Option<string>("-id|--identifier <LOGIN>", "The client identifier", CommandOptionType.SingleValue);
                idOption.DefaultValue = "clientId";
                var topicMessageOption = publishMsgCmd.Option<string>("-t|--topic <LOGIN>", "The topic message", CommandOptionType.SingleValue);
                topicMessageOption.IsRequired();
                var messageOption = publishMsgCmd.Option<string>("-m|--message <MESSAGE>", "Message", CommandOptionType.SingleValue);
                messageOption.IsRequired();
                var urlOptions = publishMsgCmd.Option<string>("-u|--url <URL>", "EventMesh URL", CommandOptionType.SingleValue);
                urlOptions.DefaultValue = Constants.DefaultUrl;
                var portOptions = publishMsgCmd.Option<int>("-p|--port <PORT>", "EventMesh Port", CommandOptionType.SingleValue);
                portOptions.DefaultValue = Constants.DefaultPort;
                publishMsgCmd.OnExecuteAsync(async (token) =>
                {
                    var evtMeshClient = new EventMeshClient(urlOptions.ParsedValue, portOptions.ParsedValue);
                    try
                    {
                        var pubSessionResult = await evtMeshClient.CreatePubSession(vpnOption.ParsedValue, idOption.ParsedValue, null, cancellationToken: token);
                        CloudEvent ce = new CloudEvent
                        {
                            Type = "com.github.pull.create",
                            Source = new Uri("https://github.com/cloudevents/spec/pull"),
                            Subject = "123",
                            Id = "A234-1234-1234",
                            Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
                            DataContentType = "application/json",
                            Data = messageOption.ParsedValue
                        };
                        await pubSessionResult.Publish(topicMessageOption.ParsedValue, ce);
                        await pubSessionResult.Disconnect(token);
                        DisplaySuccess("Message has been published");
                    }
                    catch (RuntimeClientResponseException ex)
                    {
                        DisplayError(ex);
                    }
                });
            });
        }

        private static void AddReadMessageConfiguration(CommandLineApplication app)
        {
            app.Command("read_message", readMsgCmd =>
            {
                readMsgCmd.Description = "Read one message";
                var vpnOption = readMsgCmd.Option<string>("-v|--vpn <VPN>", "The VPN", CommandOptionType.SingleValue);
                vpnOption.DefaultValue = Constants.DefaultVpn;
                var idOption = readMsgCmd.Option<string>("-id|--identifier <LOGIN>", "The client identifier", CommandOptionType.SingleValue);
                idOption.DefaultValue = "clientId";
                var topicFilterOption = readMsgCmd.Option<string>("-t|--topic <LOGIN>", "The topic message filter", CommandOptionType.SingleValue);
                topicFilterOption.IsRequired();
                var urlOptions = readMsgCmd.Option<string>("-u|--url <URL>", "EventMesh URL", CommandOptionType.SingleValue);
                urlOptions.DefaultValue = Constants.DefaultUrl;
                var portOptions = readMsgCmd.Option<int>("-p|--port <PORT>", "EventMesh Port", CommandOptionType.SingleValue);
                portOptions.DefaultValue = Constants.DefaultPort;
                readMsgCmd.OnExecuteAsync(async (token) =>
                {
                    var evtMeshClient = new EventMeshClient(urlOptions.ParsedValue, portOptions.ParsedValue);
                    try
                    {
                        var subscriptionResult = await evtMeshClient.CreateSubSession(vpnOption.ParsedValue, idOption.ParsedValue, cancellationToken: token);
                        CloudEvent ce = null;
                        subscriptionResult.DirectSubscribe(topicFilterOption.ParsedValue, (cb) =>
                        {
                            ce = cb;
                        }, token);
                        var maxRetry = 20;
                        int nbRetry = 0;
                        while(nbRetry < maxRetry)
                        {
                            if(ce != null)
                            {
                                Console.WriteLine($"Message received : {ce.Data}");
                                await subscriptionResult.Disconnect();
                                return;
                            }

                            Thread.Sleep(200);
                            nbRetry++;
                        }

                        Console.WriteLine("There is no message");
                        await subscriptionResult.Disconnect();
                    }
                    catch (RuntimeClientResponseException ex)
                    {
                        DisplayError(ex);
                    }
                });
            });
        }

        private static void DisplayError(RuntimeClientResponseException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error code = {ex.Error.Code}");
            Console.ResetColor();
        }

        private static void DisplaySuccess(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }
}
