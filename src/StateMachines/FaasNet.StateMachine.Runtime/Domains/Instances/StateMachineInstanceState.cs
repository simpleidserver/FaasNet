using Coeus;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.StateMachine.Runtime.Domains.Instances
{
    public class StateMachineInstanceState : ICloneable
    {
        public StateMachineInstanceState()
        {
            Events = new List<StateMachineInstanceStateEvent>();
            Histories = new List<StateMachineInstanceStateHistory>();
        }

        #region Properties

        public string Id { get; set; }
        public string DefId { get; set; }
        public StateMachineInstanceStateStatus Status { get; set; }
        public string InputStr { get; set; }
        public JToken GetInput()
        {
            if (string.IsNullOrWhiteSpace(InputStr))
            {
                return null;
            }

            return JObject.Parse(InputStr);
        }
        public string OutputStr { get; set; }
        public JToken GetOutput()
        {
            if (string.IsNullOrWhiteSpace(OutputStr))
            {
                return null;
            }

            return JObject.Parse(OutputStr);
        }
        public virtual ICollection<StateMachineInstanceStateHistory> Histories { get; set; }
        public virtual ICollection<StateMachineInstanceStateEvent> Events { get; set; }

        #endregion

        #region Commands

        public void Start(JToken input)
        {
            InputStr = input.ToString();
            Status = StateMachineInstanceStateStatus.ACTIVE;
            Histories.Add(StateMachineInstanceStateHistory.Create(Status, DateTime.UtcNow));
        }

        public void Complete(JToken output)
        {
            OutputStr = output == null ? string.Empty : output.ToString();
            Status = StateMachineInstanceStateStatus.COMPLETE;
            Histories.Add(StateMachineInstanceStateHistory.Create(Status, DateTime.UtcNow));
        }

        public void Block()
        {
            Status = StateMachineInstanceStateStatus.PENDING;
            Histories.Add(StateMachineInstanceStateHistory.Create(Status, DateTime.UtcNow));
        }

        public void Error(string exception)
        {
            Status = StateMachineInstanceStateStatus.ERROR;
            Histories.Add(StateMachineInstanceStateHistory.Create(Status, DateTime.UtcNow, exception));
        }

        public void AddEvent(string name, string source, string type, string topic)
        {
            Events.Add(StateMachineInstanceStateEvent.Create(name, source, type, topic));
        }

        #endregion

        #region Getters

        public StateMachineInstanceStateEvent GetConsumedEvt(string name)
        {
            return Events.FirstOrDefault(e => e.State == StateMachineInstanceStateEventStates.CONSUMED && e.Name == name);
        }

        public IEnumerable<StateMachineInstanceStateEvent> GetConsumedEvts(IEnumerable<string> names)
        {
            return Events.Where(e => e.State == StateMachineInstanceStateEventStates.CONSUMED && names.Contains(e.Name));
        }

        public IEnumerable<StateMachineInstanceStateEvent> GetProcessedEvts(IEnumerable<string> names)
        {
            return Events.Where(e => e.State == StateMachineInstanceStateEventStates.PROCESSED && names.Contains(e.Name));
        }

        public bool TryGetEvent(string name, out StateMachineInstanceStateEvent stateEvt)
        {
            stateEvt = Events.FirstOrDefault(e => e.Name == name);
            return stateEvt != null;
        }

        public bool TryGetEvent(string source, string type, out StateMachineInstanceStateEvent stateEvt)
        {
            stateEvt = Events.FirstOrDefault(e => e.Source == source && e.Type == type);
            return stateEvt != null;
        }

        public StateMachineInstanceStateEvent GetEvent(string source, string type)
        {
            return Events.FirstOrDefault(e => e.Source == source && e.Type == type);
        }

        public bool IsEvtConsumed(string name)
        {
            return IsAllEvts(new List<string> { name }, StateMachineInstanceStateEventStates.CONSUMED);
        }

        public bool IsAllEvtsConsumed(IEnumerable<string> names)
        {
            return IsAllEvts(names, StateMachineInstanceStateEventStates.CONSUMED);
        }

        public bool IsAllEvtsProcessed(IEnumerable<string> names)
        {
            return IsAllEvts(names, StateMachineInstanceStateEventStates.PROCESSED);
        }

        public bool EvaluateCondition(string expression)
        {
            expression = JTokenExtensions.CleanExpression(expression);
            var token = JQ.EvalToToken(expression, GetInput());
            return bool.Parse(token.ToString());
        }

        #endregion

        public static StateMachineInstanceState Create(string id, string defId)
        {
            return new StateMachineInstanceState
            {
                Id = id,
                DefId = defId,
                Status = StateMachineInstanceStateStatus.CREATE
            };
        }

        private bool IsAllEvts(IEnumerable<string> names, StateMachineInstanceStateEventStates state)
        {
            return names.All(n => Events.Any(e => e.Name == n && e.State == state));
        }

        public object Clone()
        {
            return new StateMachineInstanceState
            {
                DefId = DefId,
                Id = Id,
                OutputStr = OutputStr,
                Status = Status,
                InputStr = InputStr,
                Events = Events.Select(e => (StateMachineInstanceStateEvent)e.Clone()).ToList(),
                Histories = Histories.Select(h => (StateMachineInstanceStateHistory)h.Clone()).ToList()
            };
        }

        private class ConditionExpressionContext
        {
            private readonly StateMachineInstanceState _state;

            public ConditionExpressionContext(StateMachineInstanceState state)
            {
                _state = state;
            }

            public int? GetIntFromState(string jsonExpression)
            {
                var token = _state.GetInput().SelectToken(jsonExpression);
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
