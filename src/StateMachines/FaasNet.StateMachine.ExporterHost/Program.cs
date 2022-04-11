using FaasNet.Common;
using FaasNet.StateMachine.Exporter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;

namespace FaasNet.StateMachine.ExporterHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            var config = builder.Build();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddStateMachineExporter()
                .UseEventStoreDB(opt =>
                {
                    opt.ConnectionString = config["EventStoreDBConnectionString"];
                })
                .UseStateMachineInstanceElasticSearchStore(opt =>
                {
                    var connectionSettings = new ConnectionSettings(new Uri(config["ElasticSearchUrl"]));
                    opt.Settings = connectionSettings;
                });
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var hostedJob = serviceProvider.GetRequiredService<IProjectionHostedJob>();
            hostedJob.Start();
            Console.WriteLine("Please press enter to quit the application...");
            Console.ReadLine();
            hostedJob.Stop();
        }
    }
}
