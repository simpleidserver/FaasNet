﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FaasNet.Gateway.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace FaasNet.Gateway.SqlServer.Startup
{
    public class GatewayDBContextMigration : IDesignTimeDbContextFactory<GatewayDBContext>
    {
        public GatewayDBContext CreateDbContext(string[] args)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var builder = new DbContextOptionsBuilder<GatewayDBContext>();
            builder.UseSqlServer("Data Source=DESKTOP-F641MIJ\\SQLEXPRESS;Initial Catalog=Runtime;Integrated Security=True",
                optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
            return new GatewayDBContext(builder.Options);
        }
    }
}