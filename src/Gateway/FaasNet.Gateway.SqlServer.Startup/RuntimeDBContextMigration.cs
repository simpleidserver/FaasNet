// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FaasNet.Runtime.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace FaasNet.Gateway.SqlServer.Startup
{
    public class RuntimeDBContextMigration : IDesignTimeDbContextFactory<RuntimeDBContext>
    {
        public RuntimeDBContext CreateDbContext(string[] args)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<RuntimeDBContext>();
            builder.UseSqlServer("Data Source=DESKTOP-F641MIJ\\SQLEXPRESS;Initial Catalog=Runtime;Integrated Security=True",
                optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new RuntimeDBContext(builder.Options);
        }
    }
}
