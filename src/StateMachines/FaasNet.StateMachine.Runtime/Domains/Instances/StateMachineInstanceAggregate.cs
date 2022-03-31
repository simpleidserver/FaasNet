using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Domains.IntegrationEvents;
using FaasNet.StateMachine.Runtime.Exceptions;
using FaasNet.StateMachine.Runtime.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.StateMachine.Runtime.Domains.Instances
{
    public class StateMachineInstanceAggregate : ICloneable
    {
        public StateMachineInstanceAggregate()
        {
            States = new List<StateMachineInstanceState>();
            IntegrationEvents = new List<IntegrationEvent>();
        }

        #region Properties

        public string Id { get; set; }
        public string Vpn { get; set; }
        public string WorkflowDefTechnicalId { get; set; }
        public string WorkflowDefId { get; set; }
        public string WorkflowDefName { get; set; }
        public string WorkflowDefDescription { get; set; }
        public int WorkflowDefVersion { get; set; }
        public DateTime CreateDateTime { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public StateMachineInstanceStatus Status { get; set; }
        public virtual ICollection<StateMachineInstanceState> States { get; set; }
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

        public StateMachineInstanceState GetState(string id)
        {
            return States.FirstOrDefault(s => s.Id == id);
        }

        public StateMachineInstanceState GetStateByDefId(string defId)
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

            if (state.Status != StateMachineInstanceStateStatus.ACTIVE && state.Status != StateMachineInstanceStateStatus.PENDING)
            {
                throw new BusinessException(Global.AddEventToActiveState);
            }

            if (!state.TryGetEvent(name, out StateMachineInstanceStateEvent evt))
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
            if (state.Status == StateMachineInstanceStateStatus.PENDING)
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

            if (state.Status != StateMachineInstanceStateStatus.ACTIVE && state.Status != StateMachineInstanceStateStatus.PENDING)
            {
                return;
            }

            if (state.TryGetEvent(source, type, out StateMachineInstanceStateEvent evt))
            {
                if (evt.State == StateMachineInstanceStateEventStates.CREATED)
                {
                    evt.State = StateMachineInstanceStateEventStates.CONSUMED;
                    evt.InputData = data;
                }
            }
        }

        public void ProcessEvent(string stateId, string source, string type)
        {
            var state = States.First(s => s.Id == stateId);
            var evt = state.GetEvent(source, type);
            evt.State = StateMachineInstanceStateEventStates.PROCESSED;
        }

        public void AddProcessEvent(string stateId, string source, string type, int index, string output)
        {
            var state = States.First(s => s.Id == stateId);
            var evt = state.GetEvent(source, type);
            evt.OutputLst.Add(new StateMachineInstanceStateEventOutput
            {
                Data = output,
                Index = index
            });
        }

        #endregion

        public void Terminate(JToken output)
        {
            OutputStr = output.ToString();
            Status = StateMachineInstanceStatus.TERMINATE;
        }

        #endregion

        #region Helpers

        public StateMachineInstanceState AddState(string defId)
        {
            var result = StateMachineInstanceState.Create(defId);
            States.Add(result);
            return result;
        }

        #endregion

        public static StateMachineInstanceAggregate Create(string workflowDefTechnicalId, string workflowDefId, string workflowDefName, string workflowDefDescription, int workflowDefVersion, string vpn)
        {
            return new StateMachineInstanceAggregate
            {
                CreateDateTime = DateTime.UtcNow,
                WorkflowDefTechnicalId = workflowDefTechnicalId,
                Id = Guid.NewGuid().ToString(),
                WorkflowDefName = workflowDefName,
                WorkflowDefDescription = workflowDefDescription,
                WorkflowDefId = workflowDefId,
                WorkflowDefVersion = workflowDefVersion,
                Status = StateMachineInstanceStatus.ACTIVE,
                Vpn = vpn
            };
        }

        public object Clone()
        {
            return new StateMachineInstanceAggregate
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
                Vpn = Vpn,
                States = States.Select(s => (StateMachineInstanceState)s.Clone()).ToList()
            };
        }
    }
}
