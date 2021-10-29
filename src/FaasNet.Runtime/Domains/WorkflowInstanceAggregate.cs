using FaasNet.Runtime.Domains.Enums;
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
        }

        public string Id { get; set; }
        public string WorkflowDefId { get; set; }
        public string WorkflowDefVersion { get; set; }
        public DateTime CreateDateTime { get; set; }
        public WorkflowInstanceStatus Status { get; set; }
        public ICollection<WorkflowInstanceState> States { get; set; }
        public ICollection<WorkflowInstanceSequenceFlow> Flows { get; set; }
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

        public bool TryGetFirstCreateState(out WorkflowInstanceState result)
        {
            return TryGetFirstState(out result, WorkflowInstanceStateStatus.CREATE);
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

        public void Terminate(JObject output)
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

        private bool TryGetFirstState(out WorkflowInstanceState result, WorkflowInstanceStateStatus state)
        {
            result = null;
            var rootState = GetRootState();
            if (rootState.Status == state)
            {
                result = rootState;
                return true;
            }

            var rootFlow = GetRootFlow();
            return TryGetState(rootFlow, out result, state);
        }

        private bool TryGetState(WorkflowInstanceSequenceFlow flow, out WorkflowInstanceState result, WorkflowInstanceStateStatus status)
        {
            result = null;
            var state = GetState(flow.ToStateId);
            if (state.Status == status)
            {
                result = state;
                return true;
            }

            flow = GetFlow(flow.ToStateId);
            if (flow == null)
            {
                return false;
            }

            return TryGetState(flow, out result, status);
        }

        private WorkflowInstanceSequenceFlow GetFlow(string id)
        {
            return Flows.FirstOrDefault(f => f.FromStateId == id);
        }

        private WorkflowInstanceSequenceFlow GetRootFlow()
        {
            var rootState = GetRootState();
            if (rootState == null)
            {
                return null;
            }

            return GetFlow(rootState.Id);
        }

        private WorkflowInstanceState GetRootState()
        {
            return States.FirstOrDefault(s => !Flows.Any(f => f.ToStateId == s.Id));
        }

        private WorkflowInstanceState GetState(string id)
        {
            return States.FirstOrDefault(s => s.Id == id);
        }
    }
}
