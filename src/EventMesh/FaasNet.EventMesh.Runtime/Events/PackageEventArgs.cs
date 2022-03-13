using FaasNet.EventMesh.Runtime.Messages;
using System;

namespace FaasNet.EventMesh.Runtime.Events
{
    public class PackageEventArgs : EventArgs
    {
        public PackageEventArgs(Package pkg)
        {
            Package = pkg;
        }

        public Package Package { get; set; }
    }
}
