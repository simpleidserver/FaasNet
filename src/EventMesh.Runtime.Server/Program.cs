using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using EventMesh.Runtime.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;

namespace EventMesh.Runtime.Server
{
    class Program
    {
        static int Main(string[] args)
        {
            LaunchFirstEventMeshServer();
            LaunchSecondEventMeshServer();
            Console.WriteLine("Press enter to quit the application");
            Console.ReadLine();
            return 1;
        }

        private static IRuntimeHost LaunchFirstEventMeshServer()
        {
            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
            Console.WriteLine("Launch EventMesh server...");
            var path = Path.Combine(Environment.CurrentDirectory, "FirstRuntime.db");
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 4000;
            })
                .AddEF(opt => opt.UseSqlite($"Data Source={path}", optionsBuilders => optionsBuilders.MigrationsAssembly(migrationsAssembly)));
            Migrate(builder);
            var runtimeHost = builder.Build();
            runtimeHost.Run();
            Console.WriteLine("EventMesh server is launched !");
            return runtimeHost;
        }

        private static IRuntimeHost LaunchSecondEventMeshServer()
        {
            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
            Console.WriteLine("Launch EventMesh server...");
            var path = Path.Combine(Environment.CurrentDirectory, "SecondRuntime.db");
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = 4001;
                opt.Urn = "second.eventmesh.io";
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