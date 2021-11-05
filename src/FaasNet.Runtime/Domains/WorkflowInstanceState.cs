using FaasNet.Runtime.Domains.Enums;
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
            OnEvents = new List<WorkflowInstanceStateOnEvent>();
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
        public ICollection<WorkflowInstanceStateOnEvent> OnEvents { get; set; }

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

        public void AddOnEventResult(int onEventId, string eventName, JObject jObj)
        {
            OnEvents.Add(WorkflowInstanceStateOnEvent.Create(onEventId, eventName, jObj.ToString()));
        }

        #endregion

        #region Getters

        public IEnumerable<WorkflowInstanceStateEvent> GetConsumedEvts(IEnumerable<string> names)
        {
            return Events.Where(e => e.State == WorkflowInstanceStateEventStates.CONSUMED && names.Contains(e.Name));
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

        public bool IsAllEvtsConsumed(IEnumerable<string> names)
        {
            return IsAllEvts(names, WorkflowInstanceStateEventStates.CONSUMED);
        }

        public bool IsAllEvtsProcessed(IEnumerable<string> names)
        {
            return IsAllEvts(names, WorkflowInstanceStateEventStates.PROCESSED);
        }

        public JObject BuildOutputEventResult(int onEventId)
        {
            var result = new JObject();
            var dataLst = OnEvents.Where(e => e.OnEventId == onEventId).Select(e => e.Data);
            foreach(var d in dataLst)
            {
                result.Merge(d, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
            }

            return result;
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

        private bool IsAllEvts(IEnumerable<string> names, WorkflowInstanceStateEventStates state)
        {
            return names.All(n => Events.Any(e => e.Name == n && e.State == state));
        }
    }
}
