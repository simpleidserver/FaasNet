using FaasNet.Common;
using FaasNet.DHT.Chord.Client;
using FaasNet.DHT.Chord.Core;

namespace FaasNet.DHT.Chord.Service
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            /*
            var tt = HashHelper.ComputePeerId("localhost", 5000, 5);
            var tt2 = HashHelper.ComputePeerId("localhost", 5001, 5);
            var tt3 = HashHelper.ComputePeerId("localhost", 5003, 5);
            */
            var serverBuilder = new ServerBuilder().AddDHTChord().ServiceProvider.GetService(typeof(IDHTPeerFactory)) as IDHTPeerFactory;
            var firstNode = serverBuilder.Build();
            var secondNode = serverBuilder.Build();
            firstNode.Start(new DHTOptions
            {
                Port = 5000
            }, CancellationToken.None);
            secondNode.Start(new DHTOptions
            {
                Port = 5001
            }, CancellationToken.None);
            Console.WriteLine("Peer 5000 is going to join Peer 5001");
            Console.ReadLine();
            using (var chordClient = new ChordClient("localhost", 5000))
            {
                chordClient.Join("localhost", 5001);
            }

            return 1;
        }
    }
}
