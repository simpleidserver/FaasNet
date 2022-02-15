using EventMesh.Runtime.EF;
using EventMesh.Runtime.MessageBroker;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace EventMesh.Runtime.Website
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // LaunchInMemoryEventMeshServer();
            LaunchAMQPEventMeshServer();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


        private static IRuntimeHost LaunchInMemoryEventMeshServer()
        {
            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
            Console.WriteLine("Launch EventMesh server...");
            var path = Path.Combine(Environment.CurrentDirectory, "Runtime.db");
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Urn = "localhost";
                opt.Port = 4000;
            })
                .AddInMemoryMessageBroker(new List<InMemoryTopic> { new InMemoryTopic { TopicName = "firstTopic" } })
                .AddEF(opt => opt.UseSqlite($"Data Source={path}", optionsBuilders => optionsBuilders.MigrationsAssembly(migrationsAssembly)));
            Migrate(builder);
            var runtimeHost = builder.Build();
            runtimeHost.Run();
            Console.WriteLine("EventMesh server is launched !");
            return runtimeHost;
        }

        private static IRuntimeHost LaunchAMQPEventMeshServer()
        {
            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
            Console.WriteLine("Launch EventMesh server...");
            var path = Path.Combine(Environment.CurrentDirectory, "Runtime.db");
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Urn = "localhost";
                opt.Port = 4000;
            })
                .AddAMQP()
                .AddEF(opt => opt.UseSqlite($"Data Source={path}", optionsBuilders => optionsBuilders.MigrationsAssembly(migrationsAssembly)));
            Migrate(builder);
            var runtimeHost = builder.Build();
            runtimeHost.Run();
            Console.WriteLine("EventMesh server is launched !");
            return runtimeHost;
        }

        private static void Migrate(RuntimeHostBuilder runtimeHostBuilder)
        {
            var serviceProvider = runtimeHostBuilder.ServiceCollection.BuildServiceProvider();
            var dbContext = serviceProvider.GetRequiredService<EventMeshDBContext>();
            dbContext.Database.Migrate();
        }
    }
}
