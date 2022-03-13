namespace FaasNet.Domain
{
    public class BaseSearchParameter
    {
        public int StartIndex { get; set; }
        public int Count { get; set; }
        public string OrderBy { get; set; }
        public SortOrders Order { get; set; }
    }
}
