using FaasNet.EventMesh.Runtime.Models;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Core.ApplicationDomains.Queries.Results
{
    public class ApplicationResult
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Version { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public ICollection<ApplicationLinkResult> Links { get; set; }

        public Application ToDomain()
        {
            return new Application
            {
                Id = Id,
                Version = Version,
                ClientId = ClientId,
                Description = Description,
                PosX = PosX,
                PosY = PosY,
                Title = Title,
                Links = Links?.Select(l => l.ToDomain()).ToList()
            };
        }

        public static ApplicationResult Build(Application application, ApplicationDomain applicationDomain)
        {
            var result = new ApplicationResult
            {
                Id = application.Id,
                ClientId = application.ClientId,
                Description = application.Description,
                Version = application.Version,
                PosX = application.PosX,
                PosY = application.PosY,
                Title = application.Title,
                Links = application.Links?.Select(l => ApplicationLinkResult.Build(l, applicationDomain)).ToList()
            };

            return result;
        }
    }
}
