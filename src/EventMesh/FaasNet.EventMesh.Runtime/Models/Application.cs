using System.Collections.Generic;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class Application
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Version { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public ICollection<ApplicationLink> Links { get; set; }
    }
}
