using MassTransit;
using Saunter.Attributes;
using System.Threading.Tasks;

namespace FaasNet.Bus.Startup.Consumers
{
    [AsyncApi]
    public class ClientConsumer : IConsumer<ClientAddedEvent>
    {
        [Channel("clients",
            BindingsRef = "addClientChannel",
            Servers = new string[] { "rabbitmq" })]
        [PublishOperation(typeof(ClientAddedEvent),
            Summary = "Add client.",
            BindingsRef = "addClientOperation")]
        public Task Consume(ConsumeContext<ClientAddedEvent> context)
        {
            return Task.CompletedTask;
        }
    }
}
