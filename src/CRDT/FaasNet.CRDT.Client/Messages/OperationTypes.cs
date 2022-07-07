namespace FaasNet.CRDT.Client.Messages
{
    public class OperationTypes
    {
        public static OperationTypes UPDATE = new OperationTypes("UPDATE", "Update the entity");
        public static OperationTypes DELETE = new OperationTypes("DELETE", "Delete the entity");

        public OperationTypes(string name, string description)
        {

        }

        public string Name { get; set; }
        public string Description { get; set; } 
    }
}
