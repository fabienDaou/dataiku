using Microsoft.EntityFrameworkCore;
using MilleniumFalconChallenge.Persistence.MilleniumFalcon;

namespace MilleniumFalconChallenge.Tests
{
    public class TestsDbContextFactory : IDbContextFactory<RoutesDbContext>
    {
        public RoutesDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<RoutesDbContext>()
                .UseSqlite("Data Source=universe.db")
                .Options;
            return new RoutesDbContext(options);
        }
    }
}