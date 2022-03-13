using Newtonsoft.Json.Linq;
using System;

namespace FaasNet.StateMachine.Runtime.Domains.Instances
{
    public class StateMachineInstanceStateEventOutput : ICloneable
    {
        #region Properties

        public int Index { get; set; }
        public string Data { get; set; }
        public JToken DataObj
        {
            get
            {
                return JToken.Parse(Data);
            }
        }

        #endregion

        public object Clone()
        {
            return new StateMachineInstanceStateEventOutput
            {
                Index = Index,
                Data = Data
            };
        }
    }
}
