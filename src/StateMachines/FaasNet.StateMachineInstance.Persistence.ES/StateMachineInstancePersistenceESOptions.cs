using Nest;
using System;

namespace FaasNet.StateMachineInstance.Persistence.ES
{
    public class StateMachineInstancePersistenceESOptions
    {
        public StateMachineInstancePersistenceESOptions()
        {
            Settings = new ConnectionSettings(new Uri("http://localhost:9200"));
            IndexName = "StateMachineInstance";
        }

        public ConnectionSettings Settings { get; set; }
        public string IndexName { get; set; }
    }
}
