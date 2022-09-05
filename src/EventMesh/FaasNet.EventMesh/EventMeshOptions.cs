namespace FaasNet.EventMesh
{
    public class EventMeshOptions
    {
        /// <summary>
        /// Number of partitions of a topic.
        /// </summary>
        public int NbPartitionsTopic { get; set; } = 2;
        /// <summary>
        /// Maximum number of threads.
        /// </summary>
        public int MaxNbThreads { get; set; } = 5;
        /// <summary>
        /// Expiration time of a client request.
        /// </summary>
        public int RequestExpirationTimeMS { get; set; } = 5000;
    }
}
