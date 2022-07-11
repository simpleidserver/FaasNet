using FaasNet.CRDT.Core.Entities;

namespace FaasNet.DHT.Chord.Service
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            UseGCounter();
            // https://github.com/cloudstateio/cloudstate/blob/16ea6f8f17c8f8b5959e626dc4e9808d9288dae6/node-support/src/crdts/pncounter.js
            // https://mwhittaker.github.io/consistency_in_distributed_systems/3_crdt.html
            return 1;
        }

        private static void UseGCounter()
        {
            var id1 = new GCounter("id1");
            var id2 = new GCounter("id2");
            var id1FirstDelta = id1.Increment().ResetAndGetDelta();
            var id1SecondDelta = id1.Increment().ResetAndGetDelta();
            var id2FirstDelta = id2.Increment().ResetAndGetDelta();
            var id2SecondDelta=  id2.Increment().ResetAndGetDelta();
            id2.ApplyDelta("id1", id1FirstDelta);
            id2.ApplyDelta("id1", id1SecondDelta);
            id1.ApplyDelta("id2", id2FirstDelta);
            id1.ApplyDelta("id2", id2SecondDelta);
            Console.WriteLine($"Node 1, Increment = {id1.Value}");
            Console.WriteLine($"Node 2, Increment = {id2.Value}");
        }
    }
}
