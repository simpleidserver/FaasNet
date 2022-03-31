using FaasNet.StateMachine.EF;
using FaasNet.StateMachine.Runtime.Serializer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;

namespace FaasNet.StateMachine.SqlServer.Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<RuntimeDBContext>())
                {
                    context.Database.Migrate();
                    if (!context.WorkflowDefinitions.Any())
                    {
                        var files = Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), "ServerlessWorkflows"), "*.yml").ToList();
                        var runtimeSerializer = new RuntimeSerializer();
                        foreach (var file in files)
                        {
                            var workflowDef = runtimeSerializer.DeserializeYaml(File.ReadAllText(file));
                            workflowDef.RefreshTechnicalId();
                            workflowDef.CreateDateTime = DateTime.UtcNow;
                            workflowDef.UpdateDateTime = DateTime.UtcNow;
                            workflowDef.Vpn = "default";
                            workflowDef.IsLast = true;
                            context.WorkflowDefinitions.Add(workflowDef);
                        }
                    }

                    context.SaveChanges();
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
