using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Gateway.Core.Domains
{
    public class ApiDefinitionOperationUI : ICloneable
    {
        public ApiDefinitionOperationUI()
        {
            Blocks = new List<ApiDefinitionOperationBlock>();
            UIBlocks = new List<ApiDefinitionOperationBlockUI>();
        }

        public string Html { get; set; }
        public ICollection<ApiDefinitionOperationBlock> Blocks { get; set; }
        public ICollection<ApiDefinitionOperationBlockUI> UIBlocks { get; set; }

        public object Clone()
        {
            return new ApiDefinitionOperationUI
            {
                Html = Html,
                Blocks = Blocks.Select(b => (ApiDefinitionOperationBlock)b.Clone()).ToList(),
                UIBlocks = UIBlocks.Select(b => (ApiDefinitionOperationBlockUI)b.Clone()).ToList()
            };
        }
    }
}
