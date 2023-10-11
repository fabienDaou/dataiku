using Microsoft.EntityFrameworkCore;

namespace MilleniumFalconChallenge.Persistence.Scenarios
{
    public class ScenarioDbContext : DbContext
    {
        public DbSet<ScenarioEntity> Scenarios { get; set; }
        public DbSet<ScenarioBountyHunterEntity> ScenarioBountyHunters { get; set; }
        public DbSet<BountyHunterEntity> BountyHunters { get; set; }

        public ScenarioDbContext(DbContextOptions<ScenarioDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScenarioBountyHunterEntity>().HasIndex(u => new { u.ScenarioId, u.BountyHunterId }).IsUnique();
        }
    }
}