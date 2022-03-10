using FaasNet.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaasNet.EventStore
{
    public class ProjectionBuilder
    {
        private Dictionary<Type, object> _dic;

        public ProjectionBuilder()
        {
            _dic = new Dictionary<Type, object>();
        }

        public ProjectionBuilder On<T>(Func<T, Task> callback) where T : DomainEvent
        {

            _dic.Add(typeof(T), callback);
            return this;
        }
    }
}
