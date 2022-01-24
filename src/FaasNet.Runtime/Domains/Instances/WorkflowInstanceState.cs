using Coeus;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Runtime.Domains.Instances
{
    public class WorkflowInstanceState : ICloneable
    {
        public WorkflowInstanceState()
        {
            Events = new List<WorkflowInstanceStateEvent>();
            Histories = new List<WorkflowInstanceStateHistory>();
            Parameters = new Dictionary<string, string>();
        }

        #region Properties

        public string Id { get; set; }
        public string DefId { get; set; }
        public WorkflowInstanceStateStatus Status { get; set; }
        public string InputStr { get; set; }
        public JToken Input
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
        public JToken Output
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
        public Dictionary<string, string> Parameters { get; set; }
        public virtual ICollection<WorkflowInstanceStateHistory> Histories { get; set; }
        public virtual ICollection<WorkflowInstanceStateEvent> Events { get; set; }

        #endregion

        #region Commands

        public void Start(JToken input)
        {
            InputStr = input.ToString();
            Status = WorkflowInstanceStateStatus.ACTIVE;
            Histories.Add(WorkflowInstanceStateHistory.Create(Status, DateTime.UtcNow));
        }

        public void Complete(JToken output)
        {
            OutputStr = output.ToString();
            Status = WorkflowInstanceStateStatus.COMPLETE;
            Histories.Add(WorkflowInstanceStateHistory.Create(Status, DateTime.UtcNow));
        }

        public void Block()
        {
            Status = WorkflowInstanceStateStatus.PENDING;
            Histories.Add(WorkflowInstanceStateHistory.Create(Status, DateTime.UtcNow));
        }

        public void Error(string exception)
        {
            Status = WorkflowInstanceStateStatus.ERROR;
            Histories.Add(WorkflowInstanceStateHistory.Create(Status, DateTime.UtcNow, exception));
        }

        public void AddEvent(string name, string source, string type)
        {
            Events.Add(WorkflowInstanceStateEvent.Create(name, source, type));
        }

        #endregion

        #region Getters

        public WorkflowInstanceStateEvent GetConsumedEvt(string name)
        {
            return Events.FirstOrDefault(e => e.State == WorkflowInstanceStateEventStates.CONSUMED && e.Name == name);
        }

        public IEnumerable<WorkflowInstanceStateEvent> GetConsumedEvts(IEnumerable<string> names)
        {
            return Events.Where(e => e.State == WorkflowInstanceStateEventStates.CONSUMED && names.Contains(e.Name));
        }

        public IEnumerable<WorkflowInstanceStateEvent> GetProcessedEvts(IEnumerable<string> names)
        {
            return Events.Where(e => e.State == WorkflowInstanceStateEventStates.PROCESSED && names.Contains(e.Name));
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

        public bool IsEvtConsumed(string name)
        {
            return IsAllEvts(new List<string> { name }, WorkflowInstanceStateEventStates.CONSUMED);
        }

        public bool IsAllEvtsConsumed(IEnumerable<string> names)
        {
            return IsAllEvts(names, WorkflowInstanceStateEventStates.CONSUMED);
        }

        public bool IsAllEvtsProcessed(IEnumerable<string> names)
        {
            return IsAllEvts(names, WorkflowInstanceStateEventStates.PROCESSED);
        }

        public bool EvaluateCondition(string expression)
        {
            expression = JTokenExtensions.CleanExpression(expression);
            var token = JQ.EvalToToken(expression, Input);
            return bool.Parse(token.ToString());
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

        public object Clone()
        {
            return new WorkflowInstanceState
            {
                DefId = DefId,
                Id = Id,
                OutputStr = OutputStr,
                Status = Status,
                InputStr = InputStr,
                Events = Events.Select(e => (WorkflowInstanceStateEvent)e.Clone()).ToList(),
                Histories = Histories.Select(h => (WorkflowInstanceStateHistory)h.Clone()).ToList()
            };
        }

        private class ConditionExpressionContext
        {
            private readonly WorkflowInstanceState _state;

            public ConditionExpressionContext(WorkflowInstanceState state)
            {
                _state = state;
            }

            public int? GetIntFromState(string jsonExpression)
            {
                var token = _state.Input.SelectToken(jsonExpression);
                if (token == null)
                {
                    return null;
                }

                if (int.TryParse(token.ToString(), out int result))
                {
                    return result;
                }

                return null;
            }
        }
    }
}
