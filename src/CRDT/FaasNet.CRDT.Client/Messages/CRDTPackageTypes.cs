namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTPackageTypes
    {
        public CRDTPackageTypes(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}
