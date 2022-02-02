using EventMesh.Runtime.Messages;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Handlers
{
    public class SubscribeMessageHandler : IMessageHandler
    {
        public EventMeshCommands Command => EventMeshCommands.SUBSCRIBE_REQUEST;

        public Task<EventMeshPackage> Run(EventMeshPackage package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            // Récupérer la session active.
            // Récupérer les "subscriptions" venant de la requête.
            EventMeshSubscription subscription = new EventMeshSubscription();
            foreach(var topic in subscription.Topics)
            {

            }

            // Nous avons un groupe "Dictionary<string(topic>, List<Session>>()".
            // Quand un message est reçu de RABBITMQ / KAFKA ou autre alors on va appeler la session.
            throw new System.NotImplementedException();
        }
    }
}
