using EventMesh.Runtime.MessageBroker;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

namespace EventMeshServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
            var path = Path.Combine(Environment.CurrentDirectory, "Runtime.db");
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHostedService<RuntimeHostedService>();
            RegisterInMemoryEventMeshServer(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private static IServiceCollection RegisterInMemoryEventMeshServer(IServiceCollection services)
        {
            return services.AddRuntimeWebsite(opt =>
            {
                opt.Urn = "localhost";
                opt.Port = 4000;
            }).AddInMemoryMessageBroker(new ConcurrentBag<InMemoryTopic>());
        }
    }
}
