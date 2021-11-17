using FaasNet.Runtime.EF;
using FaasNet.Runtime.Serializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FaasNet.Gateway.SqlServer.Startup
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
            services.AddRuntime()
                .AddRuntimeEF(opt =>
                {
                    opt.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("Runtime"), o => o.MigrationsAssembly(migrationsAssembly));
                });
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeDatabase(app);
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<RuntimeDBContext>())
                {
                    context.Database.Migrate();
                    var files = Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), "ServerlessWorkflows"), "*.yml").ToList();
                    var runtimeSerializer = new RuntimeSerializer();
                    foreach(var file in files)
                    {
                        var workflowDef = runtimeSerializer.DeserializeYaml(File.ReadAllText(file));
                        context.WorkflowDefinitions.Add(workflowDef);
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}
