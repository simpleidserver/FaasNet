namespace FaasNet.DHT.Chord.Core
{
    public class DHTOptions
    {
        public DHTOptions()
        {
            StabilizeTimerMS = 500;
            FixFingersTimerMS = 500;
        }

        public double StabilizeTimerMS { get; set; }
        public double FixFingersTimerMS { get; set; }
    }
}
