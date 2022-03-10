using FaasNet.EventStore.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace FaasNet.EventStore.EF
{
    public class EventStoreDBContext : DbContext
    {
        public EventStoreDBContext()
        {

        }

        public DbSet<Snapshot> Snapshots { get; set; }


    }
}
