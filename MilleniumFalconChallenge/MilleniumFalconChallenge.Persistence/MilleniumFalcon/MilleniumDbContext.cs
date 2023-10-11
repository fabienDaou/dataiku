using Microsoft.EntityFrameworkCore;

namespace MilleniumFalconChallenge.Persistence.MilleniumFalcon
{
    public class MilleniumDbContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Route> Routes { get; set; }

        public MilleniumDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Route>().HasNoKey();
        }
    }
}