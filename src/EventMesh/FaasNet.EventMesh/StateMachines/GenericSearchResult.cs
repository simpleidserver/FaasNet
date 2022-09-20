using FaasNet.RaftConsensus.Core.StateMachines;
using System.Collections.Generic;

namespace FaasNet.EventMesh.StateMachines
{
    public class GenericSearchResult<T> where T : IRecord
    {
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Records { get; set; } = new List<T>();
    }
}
