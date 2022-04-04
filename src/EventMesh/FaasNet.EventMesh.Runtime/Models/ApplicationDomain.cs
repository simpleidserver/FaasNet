using FaasNet.Domain;
using FaasNet.EventMesh.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class ApplicationDomain : AggregateRoot
    {
        public ApplicationDomain()
        {
            Applications = new List<Application>();
            IntegrationEvents = new List<IntegrationEvent>();
        }

        #region Properties

        public string Vpn { get; set; }
        public string Name { get; set; }
        public string StateMachineId { get; set; }
        public string Description { get; set; }
        public string RootTopic { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public ICollection<Application> Applications { get; set; }
        public override string Topic => "";

        #region Actions

        public void Update(List<Application> applications)
        {
            Applications = applications;
            UpdateDateTime = DateTime.UtcNow;
            var rootApplication = Applications.FirstOrDefault(a => a.IsRoot);
            if(rootApplication != null)
            {               
                var evts = rootApplication.Links == null ? new List<StateMachineEvt>() : rootApplication.Links.Select(l => new StateMachineEvt
                {
                    IsConsumed = false,
                    MessageId = l.MessageId,
                    TopicName = l.TopicName
                }).ToList();
                var consumedEvts = Applications.Where(a => a.Id != rootApplication.Id).Where(a => a.Links != null).SelectMany(a => a.Links).Where(l => l.TargetId == rootApplication.Id).Select(l => new StateMachineEvt
                {
                    IsConsumed = true,
                    MessageId = l.MessageId,
                    TopicName = l.TopicName
                }).ToList();
                evts.AddRange(consumedEvts);
                if (evts.Any())
                {
                    IntegrationEvents.Add(new ApplicationDomainStateMachineEvtUpdatedEvent(Guid.NewGuid().ToString(), Id, evts));
                }
            }
        }

        #endregion

        #endregion

        public static ApplicationDomain Create(string vpn, string name, string description, string rootTopic, string stateMachineId = null)
        {
            var result = new ApplicationDomain
            {
                Id = Guid.NewGuid().ToString(),
                Vpn = vpn,
                Name = name,
                Description = description,
                RootTopic = rootTopic,
                StateMachineId = stateMachineId,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            return result;
        }

        public override void Handle(dynamic evt)
        {
        }
    }
}
