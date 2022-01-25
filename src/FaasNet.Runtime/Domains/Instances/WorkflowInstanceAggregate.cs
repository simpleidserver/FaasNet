using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Domains.IntegrationEvents;
using FaasNet.Runtime.Exceptions;
using FaasNet.Runtime.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Runtime.Domains.Instances
{
    public class WorkflowInstanceAggregate : ICloneable
    {
        public WorkflowInstanceAggregate()
        {
            States = new List<WorkflowInstanceState>();
            IntegrationEvents = new List<IntegrationEvent>();
        }

        #region Properties

        public string Id { get; set; }
        public string WorkflowDefTechnicalId { get; set; }
        public string WorkflowDefId { get; set; }
        public string WorkflowDefName { get; set; }
        public string WorkflowDefDescription { get; set; }
        public int WorkflowDefVersion { get; set; }
        public DateTime CreateDateTime { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public WorkflowInstanceStatus Status { get; set; }
        public virtual ICollection<WorkflowInstanceState> States { get; set; }
        public virtual ICollection<IntegrationEvent> IntegrationEvents { get; set; }
        public IEnumerable<EventUnlistenedEvent> EventRemovedEvts
        {
            get
            {
                return IntegrationEvents.Where(e => e is EventUnlistenedEvent).Cast<EventUnlistenedEvent>();
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

        #endregion

        #region Getters

        public WorkflowInstanceState GetState(string id)
        {
            return States.FirstOrDefault(s => s.Id == id);
        }

        public WorkflowInstanceState GetStateByDefId(string defId)
        {
            return States.FirstOrDefault(s => s.DefId == defId);
        }

        public bool TryListenEvent(string stateInstanceId, string name, string source, string type)
        {
            var state = GetState(stateInstanceId);
            if (state == null)
            {
                throw new BusinessException(string.Format(Global.UnknownWorkflowState, stateInstanceId));
            }

            if (state.Status != WorkflowInstanceStateStatus.ACTIVE && state.Status != WorkflowInstanceStateStatus.PENDING)
            {
                throw new BusinessException(Global.AddEventToActiveState);
            }

            if (!state.TryGetEvent(name, out WorkflowInstanceStateEvent evt))
            {
                state.AddEvent(name, source, type);
                var addedEvt = new EventListenedEvent(Guid.NewGuid().ToString(), Id, stateInstanceId, source, type);
                IntegrationEvents.Add(addedEvt);
                return true;
            }

            return false;
        }

        #endregion

        #region Commands

        #region Manage State

        public void StartState(string stateId, JToken input)
        {
            var state = GetState(stateId);
            state.Start(input);
        }

        public void CompleteState(string stateId, JToken output)
        {
            var state = GetState(stateId);
            state.Complete(output);
            foreach(var evt in state.Events.Select(e => new EventUnlistenedEvent(Guid.NewGuid().ToString(), Id, stateId, e.Source, e.Type)))
            {
                IntegrationEvents.Add(evt);
            }
        }

        public void BlockState(string stateId)
        {
            var state = GetState(stateId);
            if (state.Status == WorkflowInstanceStateStatus.PENDING)
            {
                return;
            }

            state.Block();
        }

        public void ErrorState(string stateId, string exception)
        {
            var state = GetState(stateId);
            state.Error(exception);
        }

        #endregion

        #region Manage Events

        public void ConsumeEvent(string stateInstanceId, string source, string type, string data)
        {
            var state = GetState(stateInstanceId);
            if (state == null)
            {
                throw new BusinessException(string.Format(Global.UnknownWorkflowState, stateInstanceId));
            }

            if (state.Status != WorkflowInstanceStateStatus.ACTIVE && state.Status != WorkflowInstanceStateStatus.PENDING)
            {
                return;
            }

            if (state.TryGetEvent(source, type, out WorkflowInstanceStateEvent evt))
            {
                if (evt.State == WorkflowInstanceStateEventStates.CREATED)
                {
                    evt.State = WorkflowInstanceStateEventStates.CONSUMED;
                    evt.InputData = data;
                }
            }
        }

        public void ProcessEvent(string stateId, string source, string type)
        {
            var state = States.First(s => s.Id == stateId);
            var evt = state.GetEvent(source, type);
            evt.State = WorkflowInstanceStateEventStates.PROCESSED;
        }

        public void AddProcessEvent(string stateId, string source, string type, int index, string output)
        {
            var state = States.First(s => s.Id == stateId);
            var evt = state.GetEvent(source, type);
            evt.OutputLst.Add(new WorkflowInstanceStateEventOutput
            {
                Data = output,
                Index = index
            });
        }

        #endregion

        public void Terminate(JToken output)
        {
            OutputStr = output.ToString();
            Status = WorkflowInstanceStatus.TERMINATE;
        }

        #endregion

        #region Helpers

        public WorkflowInstanceState AddState(string defId)
        {
            var result = WorkflowInstanceState.Create(defId);
            States.Add(result);
            return result;
        }

        #endregion

        public static WorkflowInstanceAggregate Create(string workflowDefTechnicalId, string workflowDefId, string workflowDefName, string workflowDefDescription, int workflowDefVersion)
        {
            return new WorkflowInstanceAggregate
            {
                CreateDateTime = DateTime.UtcNow,
                WorkflowDefTechnicalId = workflowDefTechnicalId,
                Id = Guid.NewGuid().ToString(),
                WorkflowDefName = workflowDefName,
                WorkflowDefDescription = workflowDefDescription,
                WorkflowDefId = workflowDefId,
                WorkflowDefVersion = workflowDefVersion,
                Status = WorkflowInstanceStatus.ACTIVE
            };
        }

        public object Clone()
        {
            return new WorkflowInstanceAggregate
            {
                CreateDateTime = CreateDateTime,
                Id = Id,
                OutputStr = OutputStr,
                WorkflowDefTechnicalId = WorkflowDefTechnicalId,
                Status = Status,
                WorkflowDefVersion = WorkflowDefVersion,
                WorkflowDefId = WorkflowDefId,
                WorkflowDefName = WorkflowDefName,
                WorkflowDefDescription = WorkflowDefDescription,
                States = States.Select(s => (WorkflowInstanceState)s.Clone()).ToList()
            };
        }
    }
}
