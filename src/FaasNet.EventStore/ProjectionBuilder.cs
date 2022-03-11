using FaasNet.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FaasNet.EventStore
{
    public class ProjectionBuilder
    {
        private Dictionary<Type, Func<object, Task>> _dic;

        public ProjectionBuilder()
        {
            _dic = new Dictionary<Type, Func<object, Task>>();
        }

        public ProjectionBuilder On<T>(Func<T, Task> callback) where T : DomainEvent
        {
            Func<object, Task> func = (o) =>
            {
                var type = callback.GetType();
                var method = type.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
                var result = method.Invoke(callback, new object[] { o });
                return result as Task;
            };
            _dic.Add(typeof(T), func);
            return this;
        }

        public Func<object, Task> Get(Type type)
        {
            return _dic.FirstOrDefault(kvp => kvp.Key == type).Value;
        }
    }
}
