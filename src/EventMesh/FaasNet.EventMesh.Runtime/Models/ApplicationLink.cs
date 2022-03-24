namespace FaasNet.EventMesh.Runtime.Models
{
    public class ApplicationLink
    {
        public string MessageId { get; set; }
        public string TopicName { get; set; }
        public string TargetId { get; set; }
        public AnchorDirections StartAnchor { get; set; }
        public AnchorDirections EndAnchor { get; set; }
    }
}
