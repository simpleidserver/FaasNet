using EventMesh.Runtime.Messages;
using System;

namespace EventMesh.Runtime.Events
{
    public class EventMeshPackageEventArgs : EventArgs
    {
        public EventMeshPackageEventArgs(EventMeshPackage pkg)
        {
            Package = pkg;
        }

        public EventMeshPackage Package { get; set; }
    }
}
