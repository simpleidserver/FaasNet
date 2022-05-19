
using FaasNet.EventMesh.Seed;
using Microsoft.Extensions.DependencyInjection;

var seedJobs = BuildJobs();
Console.WriteLine("Press Enter to launch the jobs");
Console.ReadLine();
foreach (var seedJob in seedJobs) await seedJob.Start(CancellationToken.None);

Console.WriteLine("Press Enter to stop the jobs");
Console.ReadLine();
foreach (var seedJob in seedJobs) await seedJob.Stop();


static IEnumerable<ISeedJob> BuildJobs()
{
    var serviceProvider = new ServiceCollection()
        .AddAMQPSeed(o =>
        {
            o.EventMeshPort = 4000;
            o.EventMeshUrl = "localhost";
            o.Vpn = "default";
            o.ClientId = "publishClientId";
        }, o =>
        {
            o.JobId = "RabbitMQ";
            o.ConnectionFactory = (o) =>
            {
                o.Port = 30007;
            };
        })
        .BuildServiceProvider();
    return serviceProvider.GetRequiredService<IEnumerable<ISeedJob>>();
}