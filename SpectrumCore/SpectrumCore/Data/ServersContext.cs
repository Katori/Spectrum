using Microsoft.EntityFrameworkCore;

namespace SpectrumCore.Models
{
    public class ServersContext : DbContext
    {
        public ServersContext (DbContextOptions<ServersContext> options)
            : base(options)
        {
        }

        public DbSet<SpectrumCore.Models.Server> Server { get; set; }
    }
}
