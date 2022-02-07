using EventMesh.Runtime.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.IO;
using System.Reflection;

namespace EventMesh.Runtime.Server
{
    public class EventMeshRuntimeMigration : IDesignTimeDbContextFactory<EventMeshDBContext>
    {
        public EventMeshDBContext CreateDbContext(string[] args)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Runtime.db");
            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<EventMeshDBContext>();
            builder.UseSqlite($"Data Source={path}", optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new EventMeshDBContext(builder.Options);
        }
    }
}
