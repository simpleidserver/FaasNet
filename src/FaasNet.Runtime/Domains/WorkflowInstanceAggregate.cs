using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Domains.IntegrationEvents;
using FaasNet.Runtime.Exceptions;
using FaasNet.Runtime.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Runtime.Domains
{
    public class WorkflowInstanceAggregate
    {
        public WorkflowInstanceAggregate()
        {
            States = new List<WorkflowInstanceState>();
            Flows = new List<WorkflowInstanceSequenceFlow>();
            IntegrationEvents = new List<IntegrationEvent>();
        }

        public string Id { get; set; }
        public string WorkflowDefId { get; set; }
        public string WorkflowDefVersion { get; set; }
        public DateTime CreateDateTime { get; set; }
        public WorkflowInstanceStatus Status { get; set; }
        public ICollection<WorkflowInstanceState> States { get; set; }
        public ICollection<WorkflowInstanceSequenceFlow> Flows { get; set; }
        public ICollection<IntegrationEvent> IntegrationEvents { get; set; }
        public IEnumerable<EventAddedEvent> EventAddedEvts
        {
            get
            {
                return IntegrationEvents.Where(e => e is EventAddedEvent).Cast<EventAddedEvent>();
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

        #region Getters

        public WorkflowInstanceState GetRootState()
        {
            return States.FirstOrDefault(s => !Flows.Any(f => f.ToStateId == s.Id));
        }

        public WorkflowInstanceState GetState(string id)
        {
            return States.FirstOrDefault(s => s.Id == id);
        }

        public IEnumerable<string> GetNextStateInstanceIds(string id)
        {
            return Flows.Where(f => f.FromStateId == id).Select(f => f.ToStateId);
        }

        #endregion

        #region Commands

        public void StartState(string stateId, JObject input)
        {
            var state = GetState(stateId);
            state.Start(input);
        }

        public void CompleteState(string stateId, JObject output)
        {
            var state = GetState(stateId);
            state.Complete(output);
        }

        public void ProcessEvent(string stateId, string source, string type)
        {
            var state = States.First(s => s.Id == stateId);
            var evt = state.GetEvent(source, type);
            evt.State = WorkflowInstanceStateEventStates.PROCESSED;
        }

        public void ProcessAllEvents(string stateId)
        {
            var state = States.First(s => s.Id == stateId);
            foreach(var evt in state.Events)
            {
                evt.State = WorkflowInstanceStateEventStates.PROCESSED;
            }
        }

        public void Terminate(JObject output)
        {
            OutputStr = output.ToString();
            Status = WorkflowInstanceStatus.TERMINATE;
        }

        public bool TryListenEvent(string stateInstanceId, string name, string source, string type)
        {
            var state = GetState(stateInstanceId);
            if (state == null)
            {
                throw new BusinessException(string.Format(Global.UnknownWorkflowState, stateInstanceId));
            }

            if(state.Status != WorkflowInstanceStateStatus.ACTIVE)
            {
                throw new BusinessException(Global.AddEventToActiveState);
            }

            if (!state.TryGetEvent(name, out WorkflowInstanceStateEvent evt))
            {
                state.AddEvent(name, source, type);
                var addedEvt = new EventAddedEvent(Guid.NewGuid().ToString(), Id, stateInstanceId, source, type);
                IntegrationEvents.Add(addedEvt);
                return true;
            }

            return false;
        }

        public void ConsumeEvent(string stateInstanceId, string source, string type, string data)
        {
            var state = GetState(stateInstanceId);
            if (state == null)
            {
                throw new BusinessException(string.Format(Global.UnknownWorkflowState, stateInstanceId));
            }

            if (state.Status != WorkflowInstanceStateStatus.ACTIVE)
            {
                return;
            }

            if (state.TryGetEvent(source, type, out WorkflowInstanceStateEvent evt))
            {
                if (evt.State == WorkflowInstanceStateEventStates.CREATED)
                {
                    evt.State = WorkflowInstanceStateEventStates.CONSUMED;
                    evt.Data = data;
                }
            }
        }

        #endregion

        #region Helpers

        public WorkflowInstanceState AddState(string defId)
        {
            var result = WorkflowInstanceState.Create(defId);
            States.Add(result);
            return result;
        }

        public void AddFlow(string fromStateId, string toStateId)
        {
            Flows.Add(WorkflowInstanceSequenceFlow.Create(fromStateId, toStateId));
        }

        #endregion

        public static WorkflowInstanceAggregate Create(string workflowDefId, string workflowDefVersion)
        {
            return new WorkflowInstanceAggregate
            {
                CreateDateTime = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString(),
                WorkflowDefId = workflowDefId,
                WorkflowDefVersion = workflowDefVersion,
                Status = WorkflowInstanceStatus.ACTIVE
            };
        }
    }
}
