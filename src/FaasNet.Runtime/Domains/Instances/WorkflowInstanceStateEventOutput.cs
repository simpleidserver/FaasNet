using Newtonsoft.Json.Linq;
using System;

namespace FaasNet.Runtime.Domains.Instances
{
    public class WorkflowInstanceStateEventOutput : ICloneable
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
            return new WorkflowInstanceStateEventOutput
            {
                Index = Index,
                Data = Data
            };
        }
    }
}
