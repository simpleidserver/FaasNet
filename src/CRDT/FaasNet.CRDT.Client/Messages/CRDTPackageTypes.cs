namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTPackageTypes
    {
        public static CRDTPackageTypes DELETE = new CRDTPackageTypes("DELETE", "Delete Entity");
        public static CRDTPackageTypes DELTA = new CRDTPackageTypes("DELTA", "Apply delta");
        public static CRDTPackageTypes ERROR = new CRDTPackageTypes("ERROR", "Error");

        public CRDTPackageTypes(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}