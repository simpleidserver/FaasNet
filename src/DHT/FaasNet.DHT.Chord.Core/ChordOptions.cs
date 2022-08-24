namespace FaasNet.DHT.Chord.Core
{
    public class ChordOptions
    {
        public double StabilizeTimerMS { get; set; } = 500;
        public double FixFingersTimerMS { get; set; } = 500;
        public double CheckPredecessorAndSuccessorTimerMS { get; set; } = 500;
        public int RequestExpirationTimeMS { get; set; } = 5000;
    }
}
