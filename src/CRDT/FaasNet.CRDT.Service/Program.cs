using FaasNet.CRDT.Client;
using FaasNet.CRDT.Core.SerializedEntities;
using FaasNet.Peer;
using System.Collections.Concurrent;

namespace FaasNet.DHT.Chord.Service
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            // Pour le clock il faut stocker.
            // Quand on incrémente alors on récupère la dernière valeur.
            // Quand on ajoute dans une liste alors on récupère les N dernières valeurs.
            // UseGCounter();
            var peerHost = LaunchCRDTPeer();
            Console.WriteLine("Increment counter");
            Console.ReadLine();
            using (var crdtClient = new UDPCRDTClient("localhost", 5001))
            {
                crdtClient.IncrementGCounter("nb_customers", 2).Wait();
            }

            peerHost.Stop();
            Console.WriteLine("Press any key to quit the application");
            Console.ReadLine();
            // https://github.com/cloudstateio/cloudstate/blob/16ea6f8f17c8f8b5959e626dc4e9808d9288dae6/node-support/src/crdts/pncounter.js
            // https://mwhittaker.github.io/consistency_in_distributed_systems/3_crdt.html
            return 1;
        }

        private static IPeerHost LaunchCRDTPeer()
        {
            var entities = new ConcurrentBag<SerializedEntity>
            {
                new SerializedEntity
                {
                    Id = "nb_customers",
                    Type = "GCounter",
                    Value = null
                }
            };
            var peerHost = PeerHostFactory.New()
                .UseUDPTransport()
                .AddCRDTProtocol(entities)
                .Build();
            peerHost.Start();
            return peerHost;
        }
    }
}
