using Microsoft.EntityFrameworkCore;

namespace MilleniumFalconChallenge.Persistence.MilleniumFalcon
{
    public class MilleniumDbContext : DbContext
    {
        public DbSet<Route> Routes { get; set; }

        public MilleniumDbContext(DbContextOptions<MilleniumDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Route>().HasNoKey();
        }
    }
}