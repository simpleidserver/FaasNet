using System.Collections.Generic;

namespace FaasNet.Domain
{
    public class BaseSearchResult<T> where T : class
    {
        public BaseSearchResult()
        {
            Content = new List<T>();
        }

        public int StartIndex { get; set; }
        public int Count { get; set; }
        public int TotalLength { get; set; }
        public ICollection<T> Content { get; set; }
    }
}
