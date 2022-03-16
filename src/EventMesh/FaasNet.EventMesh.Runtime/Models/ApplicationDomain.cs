using System;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class ApplicationDomain
    {
        public string Id { get; set; }  
        public string Name { get; set; }
        public string Description { get; set; }
        public string RootTopic { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
    }
}
