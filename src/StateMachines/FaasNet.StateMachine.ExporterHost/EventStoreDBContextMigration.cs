using FaasNet.EventStore.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.IO;
using System.Reflection;

namespace FaasNet.StateMachine.ExporterHost
{
    public class EventStoreDBContextMigration : IDesignTimeDbContextFactory<EventStoreDBContext>
    {
        public EventStoreDBContext CreateDbContext(string[] args)
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StateMachineExporter.db");
            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<EventStoreDBContext>();
            builder.UseSqlite($"Data Source={dbPath}", optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new EventStoreDBContext(builder.Options);
        }
    }
}
