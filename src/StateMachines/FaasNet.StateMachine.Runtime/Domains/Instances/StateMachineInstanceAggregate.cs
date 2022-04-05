using FaasNet.Domain;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Domains.Instances.Events;
using FaasNet.StateMachine.Runtime.Exceptions;
using FaasNet.StateMachine.Runtime.IntegrationEvents;
using FaasNet.StateMachine.Runtime.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.StateMachine.Runtime.Domains.Instances
{
    public class StateMachineInstanceAggregate : AggregateRoot, ICloneable
    {
        public StateMachineInstanceAggregate()
        {
            States = new List<StateMachineInstanceState>();
        }

        #region Properties

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
        public string SerializedDefinition { get; set; }

        public override string Topic => "";

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

        #endregion

        #region Commands

        #region Manage State

        public void StartState(string stateId, JToken input)
        {
            var evt = new StateStartedEvent(Guid.NewGuid().ToString(), Id, stateId, input, DateTime.UtcNow);
            Handle(evt);
            DomainEvts.Add(evt);
        }

        public void CompleteState(string stateId, JToken output)
        {
            var evt = new StateCompletedEvent(Guid.NewGuid().ToString(), Id, stateId, output, DateTime.UtcNow);
            Handle(evt);
            DomainEvts.Add(evt);
        }

        public void BlockState(string stateId)
        {
            var evt = new StateBlockedEvent(Guid.NewGuid().ToString(), Id, stateId, DateTime.UtcNow);
            Handle(evt);
            DomainEvts.Add(evt);
        }

        public void ErrorState(string stateId, string exception)
        {
            var evt = new StateFailedEvent(Guid.NewGuid().ToString(), Id, stateId, exception, DateTime.UtcNow);
            Handle(evt);
            DomainEvts.Add(evt);
        }

        #endregion

        #region Manage Events

        public void ListenEvt(string stateInstanceId, string name, string source, string type)
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

            if (!state.TryGetEvent(name, out StateMachineInstanceStateEvent evtState))
            {
                var evt = new StateEvtListenedEvent(Guid.NewGuid().ToString(), Id, stateInstanceId, name, source, type);
                var integrationEvt = new EventListenedEvent(Guid.NewGuid().ToString(), Id, evt.StateId, Vpn, evt.EvtSource, evt.EvtType);
                Handle(evt);
                DomainEvts.Add(evt);
                IntegrationEvents.Add(integrationEvt);
            }
        }

        public void ConsumeEvt(string stateId, string source, string type, int index, JToken output)
        {
            var state = States.First(s => s.Id == stateId);
            var evt = state.GetEvent(source, type);
            evt.OutputLst.Add(new StateMachineInstanceStateEventOutput
            {
                Data = output.ToString(),
                Index = index
            });
        }

        public bool TryConsumeEvt(string stateInstanceId, string source, string type, string data)
        {
            var state = GetState(stateInstanceId);
            if (state == null)
            {
                throw new BusinessException(string.Format(Global.UnknownWorkflowState, stateInstanceId));
            }

            if (state.Status != StateMachineInstanceStateStatus.ACTIVE && state.Status != StateMachineInstanceStateStatus.PENDING)
            {
                return false;
            }

            if (state.TryGetEvent(source, type, out StateMachineInstanceStateEvent evt))
            {
                if (evt.State == StateMachineInstanceStateEventStates.CREATED)
                {
                    var domainEvt = new StateEvtConsumedEvent(Guid.NewGuid().ToString(), Id, stateInstanceId, source, type, data);
                    Handle(domainEvt);
                    DomainEvts.Add(domainEvt);
                    return true;
                }
            }

            return false;
        }

        public void ProcessEvent(string stateId, string source, string type)
        {
            var state = States.First(s => s.Id == stateId);
            var evt = state.GetEvent(source, type);
            evt.State = StateMachineInstanceStateEventStates.PROCESSED;
        }

        #endregion

        public void Terminate(JToken output)
        {
            var evt = new StateMachineTerminatedEvent(Guid.NewGuid().ToString(), Id, output, DateTime.UtcNow);
            Handle(evt);
            DomainEvts.Add(evt);
        }

        #endregion

        #region Consume Domain Events

        public override void Handle(dynamic evt)
        {
            Handle(evt);
        }

        public void Handle(StateStartedEvent evt)
        {
            var state = GetState(evt.StateId);
            state.Start(evt.Input);
        }

        public void Handle(StateBlockedEvent evt)
        {
            var state = GetState(evt.StateId);
            if (state.Status == StateMachineInstanceStateStatus.PENDING)
            {
                return;
            }

            state.Block();
        }

        public void Handle(StateFailedEvent evt)
        {
            var state = GetState(evt.StateId);
            state.Error(evt.Exception);
        }

        public void Handle(StateCompletedEvent evt)
        {
            var state = GetState(evt.StateId);
            state.Complete(evt.Output);
            /*
            foreach (var evt in state.Events.Select(e => new EventUnlistenedEvent(Guid.NewGuid().ToString(), Id, stateId, e.Source, e.Type)))
            {
                // IntegrationEvents.Add(evt);
            }
            */
        }

        public void Handle(StateMachineTerminatedEvent evt)
        {
            OutputStr = evt.Output.ToString();
            Status = StateMachineInstanceStatus.TERMINATE;
        }

        public void Handle(StateEvtListenedEvent evt)
        {
            var state = GetState(evt.StateId);
            state.AddEvent(evt.EvtName, evt.EvtSource, evt.EvtType);
        }

        public void Handle(StateEvtConsumedEvent evt)
        {
            var state = GetState(evt.StateId);
            state.TryGetEvent(evt.EvtSource, evt.EvtType, out StateMachineInstanceStateEvent stateEvt);
            stateEvt.State = StateMachineInstanceStateEventStates.CONSUMED;
            stateEvt.InputData = evt.Input;
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

        public static StateMachineInstanceAggregate Create(string workflowDefTechnicalId, string workflowDefId, string workflowDefName, string workflowDefDescription, int workflowDefVersion, string vpn, string serializedDefinition)
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
                Vpn = vpn,
                SerializedDefinition = serializedDefinition
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
                SerializedDefinition = SerializedDefinition,
                States = States.Select(s => (StateMachineInstanceState)s.Clone()).ToList()
            };
        }
    }
}
