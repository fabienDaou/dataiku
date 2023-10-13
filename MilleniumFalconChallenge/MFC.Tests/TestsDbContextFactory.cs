using MFC.Persistence.MilleniumFalcon;
using Microsoft.EntityFrameworkCore;

namespace MFC.Tests
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