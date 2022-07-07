using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaasNet.CRDT.Client.Messages
{
    public class DeleteOperationPackage : BaseOperationPackage
    {
        public override OperationTypes Type => OperationTypes.DELETE;
    }
}
