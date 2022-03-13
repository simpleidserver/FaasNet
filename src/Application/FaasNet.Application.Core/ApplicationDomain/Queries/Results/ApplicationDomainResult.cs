using System;

namespace FaasNet.Application.Core.ApplicationDomain.Queries.Results
{
    public class ApplicationDomainResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RootTopic { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
    }
}
