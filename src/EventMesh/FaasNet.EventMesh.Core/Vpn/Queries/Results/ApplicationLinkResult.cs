using FaasNet.EventMesh.Runtime.Models;
using System;
using System.Linq;

namespace FaasNet.EventMesh.Core.Vpn.Queries.Results
{
    public class ApplicationLinkResult
    {
        public string TopicName { get; set; }
        public MessageDefinitionResult Message { get; set; }
        public ApplicationResult Target { get; set; }
        public int StartAnchor { get; set; }
        public int EndAnchor { get; set; }

        public ApplicationLink ToDomain()
        {
            return new ApplicationLink
            {
                TopicName = TopicName,
                MessageId = Message?.Id,
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
                EndAnchor = (int)link.EndAnchor
            };
            if (!string.IsNullOrWhiteSpace(link.MessageId))
            {
                result.Message = MessageDefinitionResult.Build(appDomain.MessageDefinitions.First(m => m.Id == link.MessageId));
            }

            if (!string.IsNullOrWhiteSpace(link.TargetId))
            {
                result.Target = ApplicationResult.Build(appDomain.Applications.First(a => a.Id == link.TargetId), appDomain);
            }

            return result;
        }
    }
}
