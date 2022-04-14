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
        public static string TOPIC_NAME = "StateMachineInstance";

        public StateMachineInstanceAggregate()
        {
            States = new List<StateMachineInstanceState>();
        }

        #region Properties

        public string Vpn { get; set; }
        public string WorkflowDefTechnicalId { get; set; }
        public string RootTopic { get; set; }
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
        public JObject GetOutput()
        {
            if (string.IsNullOrWhiteSpace(OutputStr))
            {
                return null;
            }

            return JObject.Parse(OutputStr);
        }
        public string SerializedDefinition { get; set; }

        public override string Topic => TOPIC_NAME;

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

        public StateMachineInstanceState AddState(string defId)
        {
            var evt = new StateInstanceCreatedEvent(Guid.NewGuid().ToString(), Id, Guid.NewGuid().ToString(), defId);
            Handle(evt);
            DomainEvts.Add(evt);
            return States.Last();
        }

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

        public void ListenEvt(string stateInstanceId, string name, string source, string type, string topic)
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
                var evt = new StateEvtListenedEvent(Guid.NewGuid().ToString(), Id, stateInstanceId, name, source, type, topic);
                var integrationEvt = new EventListenedEvent(Guid.NewGuid().ToString(), Id, evt.StateId, Vpn, RootTopic, evt.EvtSource, evt.EvtType, evt.Topic);
                Handle(evt);
                DomainEvts.Add(evt);
                IntegrationEvents.Add(integrationEvt);
            }
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

        public void ProcessEvent(string stateId, string evtName, string output)
        {
            var evt = new StateProcessedEvent(Guid.NewGuid().ToString(), Id, stateId, evtName, output);
            Handle(evt);
            DomainEvts.Add(evt);
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

        public void Handle(StateMachineInstanceCreatedEvent evt)
        {
            Id = evt.AggregateId;
            WorkflowDefTechnicalId = evt.WorkflowDefTechnicalId;
            WorkflowDefName = evt.WorkflowDefName;
            WorkflowDefDescription = evt.WorkflowDefDescription;
            WorkflowDefId = evt.WorkflowDefId;
            WorkflowDefVersion = evt.WorkflowDefVersion;
            Status = StateMachineInstanceStatus.ACTIVE;
            Vpn = evt.Vpn;
            RootTopic = evt.RootTopic;
            SerializedDefinition = evt.SerializedDefinition;
            CreateDateTime = evt.CreateDateTime;
        }

        public void Handle(StateInstanceCreatedEvent evt)
        {
            States.Add(StateMachineInstanceState.Create(evt.StateInstanceId, evt.DefId));
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
            OutputStr = evt.Output == null ? null : evt.Output.ToString();
            Status = StateMachineInstanceStatus.TERMINATE;
        }

        public void Handle(StateEvtListenedEvent evt)
        {
            var state = GetState(evt.StateId);
            state.AddEvent(evt.EvtName, evt.EvtSource, evt.EvtType, evt.Topic);
        }

        public void Handle(StateEvtConsumedEvent evt)
        {
            var state = GetState(evt.StateId);
            state.TryGetEvent(evt.EvtSource, evt.EvtType, out StateMachineInstanceStateEvent stateEvt);
            stateEvt.State = StateMachineInstanceStateEventStates.CONSUMED;
            stateEvt.InputData = evt.Input;
        }

        public void Handle(StateProcessedEvent evt)
        {
            var state = States.First(s => s.Id == evt.StateId);
            var evtState = state.Events.First(e => e.Name == evt.EvtName);
            evtState.State = StateMachineInstanceStateEventStates.PROCESSED;
            evtState.OutputData = evt.Output;
        }

        #endregion

        public static StateMachineInstanceAggregate Create(string workflowDefTechnicalId, string workflowDefId, string workflowDefName, string workflowDefDescription, int workflowDefVersion, string vpn, string rootTopic, string serializedDefinition)
        {
            var evt = new StateMachineInstanceCreatedEvent(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), workflowDefTechnicalId, workflowDefId, workflowDefName, workflowDefDescription, workflowDefVersion, vpn, rootTopic, serializedDefinition, DateTime.UtcNow);
            var result = new StateMachineInstanceAggregate();
            result.Handle(evt);
            result.DomainEvts.Add(evt);
            return result;
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
