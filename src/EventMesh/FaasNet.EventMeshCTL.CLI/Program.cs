﻿using FaasNet.EventMesh.Client;
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
            AddClientCommand(app);
            AddGetAllVpnCommand(app);
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
                clusterStatusCmd.OnExecuteAsync(async (token) =>
                {
                    var configuration = EventMeshCTLConfigurationManager.Get();
                    var gossipClient = new GossipClient(configuration.Url, configuration.Port);
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
                addVpnCmd.Description = "Add VPN";
                addVpnCmd.OnExecuteAsync(async (token) =>
                {
                    var vpnName = nameOption.ParsedValue;
                    var configuration = EventMeshCTLConfigurationManager.Get();
                    var evtMeshClient = new EventMeshClient(configuration.Url, configuration.Port);
                    try
                    {
                        await evtMeshClient.AddVpn(vpnName, token);
                    }
                    catch(RuntimeClientResponseException ex)
                    {
                        DisplayError(ex);
                        return;
                    }

                    Console.WriteLine($"VPN {vpnName} has been added");
                });
            });
        }

        private static void AddGetAllVpnCommand(CommandLineApplication app)
        {
            app.Command("get_all_vpn", getAllVpnCmd =>
            {
                getAllVpnCmd.Description = "Get all VPN";
                getAllVpnCmd.OnExecuteAsync(async (token) =>
                {
                    var configuration = EventMeshCTLConfigurationManager.Get();
                    var evtMeshClient = new EventMeshClient(configuration.Url, configuration.Port);
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
                var subEnabled = addClientCmd.Option<bool>("-s|--subription_enabled <SUBSCRIPTION>", "Enable subscription", CommandOptionType.SingleValue);
                subEnabled.DefaultValue = true;
                addClientCmd.OnExecuteAsync(async (token) =>
                {
                    var configuration = EventMeshCTLConfigurationManager.Get();
                    var evtMeshClient = new EventMeshClient(configuration.Url, configuration.Port);
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

                    Console.WriteLine($"Client {idOption.ParsedValue} has been added");
                });
            });
        }

        private static void DisplayError(RuntimeClientResponseException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error code = {ex.Error.Code}");
            Console.ResetColor();
        }
    }
}
