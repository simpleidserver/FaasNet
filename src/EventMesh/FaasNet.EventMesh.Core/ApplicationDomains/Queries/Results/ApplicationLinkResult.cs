using FaasNet.EventMesh.Runtime.Models;
using System.Linq;

namespace FaasNet.EventMesh.Core.ApplicationDomains.Queries.Results
{
    public class ApplicationLinkResult
    {
        public string TopicName { get; set; }
        public string MessageId { get; set; }
        public ApplicationResult Target { get; set; }
        public int StartAnchor { get; set; }
        public int EndAnchor { get; set; }

        public ApplicationLink ToDomain()
        {
            return new ApplicationLink
            {
                TopicName = TopicName,
                MessageId = MessageId,
                TargetId = Target?.Id,
                StartAnchor = (AnchorDirections)StartAnchor,
                EndAnchor = (AnchorDirections)EndAnchor
            };
        }

        public static ApplicationLinkResult Build(ApplicationLink link, ApplicationDomain appDomain)
        {
            var result = new ApplicationLinkResult
            {
                TopicName = link.TopicName,
                StartAnchor = (int)link.StartAnchor,
                EndAnchor = (int)link.EndAnchor,
                MessageId = link.MessageId
            };
            if (!string.IsNullOrWhiteSpace(link.TargetId))
            {
                result.Target = ApplicationResult.Build(appDomain.Applications.First(a => a.Id == link.TargetId), appDomain);
            }

            return result;
        }
    }
}
