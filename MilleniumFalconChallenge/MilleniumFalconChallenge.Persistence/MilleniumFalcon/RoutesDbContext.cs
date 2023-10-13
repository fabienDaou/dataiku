using Microsoft.EntityFrameworkCore;

namespace MilleniumFalconChallenge.Persistence.MilleniumFalcon
{
    public class RoutesDbContext : DbContext
    {
        public DbSet<RouteEntity> Routes { get; set; }

        public RoutesDbContext(DbContextOptions<RoutesDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RouteEntity>().HasNoKey();
        }
    }
}