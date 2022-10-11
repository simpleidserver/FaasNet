namespace FaasNet.EventMesh.UI.ViewModels
{
    public class EventDefinitionViewModel
    {
        public string Id { get; set; } = null!;
        public DateTime CreateDateTime { get; set; }
        public string JsonSchema { get; set; } = null!;
    }
}
