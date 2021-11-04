﻿using FaasNet.Runtime.Domains.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Runtime.Domains
{
    public class WorkflowInstanceState
    {
        public WorkflowInstanceState()
        {
            Events = new List<WorkflowInstanceStateEvent>();
        }

        public string Id { get; set; }
        public string DefId { get; set; }
        public WorkflowInstanceStateStatus Status { get; set; }
        public string InputStr { get; set; }
        public JObject Input
        {
            get
            {
                if (string.IsNullOrWhiteSpace(InputStr))
                {
                    return null;
                }

                return JObject.Parse(InputStr);
            }
        }
        public string OutputStr { get; set; }
        public JObject Output
        {
            get
            {
                if (string.IsNullOrWhiteSpace(OutputStr))
                {
                    return null;
                }

                return JObject.Parse(OutputStr);
            }
        }
        public ICollection<WorkflowInstanceStateEvent> Events { get; set; }

        #region Commands

        public void Start(JObject input)
        {
            InputStr = input.ToString();
            Status = WorkflowInstanceStateStatus.ACTIVE;
        }

        public void Complete(JObject output)
        {
            OutputStr = output.ToString();
            Status = WorkflowInstanceStateStatus.COMPLETE;
        }

        public void AddEvent(string name, string source, string type)
        {
            Events.Add(WorkflowInstanceStateEvent.Create(name, source, type));
        }

        #endregion

        #region Getters

        public IEnumerable<WorkflowInstanceStateEvent> GetConsumedEvts()
        {
            return Events.Where(e => e.State == WorkflowInstanceStateEventStates.CONSUMED);
        }

        public bool TryGetEvent(string name, out WorkflowInstanceStateEvent stateEvt)
        {
            stateEvt = Events.FirstOrDefault(e => e.Name == name);
            return stateEvt != null;
        }

        public bool TryGetEvent(string source, string type, out WorkflowInstanceStateEvent stateEvt)
        {
            stateEvt = Events.FirstOrDefault(e => e.Source == source && e.Type == type);
            return stateEvt != null;
        }

        public WorkflowInstanceStateEvent GetEvent(string source, string type)
        {
            return Events.FirstOrDefault(e => e.Source == source && e.Type == type);
        }

        public bool IsAllEvtsConsumed()
        {
            return Events.All(e => e.State == WorkflowInstanceStateEventStates.CONSUMED);
        }

        public bool IsAllEvtsProcessed(IEnumerable<string> names)
        {
            return names.All(n => Events.Any(e => e.Name == n && e.State == WorkflowInstanceStateEventStates.PROCESSED));
        }

        #endregion

        public static WorkflowInstanceState Create(string defId)
        {
            return new WorkflowInstanceState
            {
                Id = Guid.NewGuid().ToString(),
                DefId = defId,
                Status = WorkflowInstanceStateStatus.CREATE
            };
        }
    }
}