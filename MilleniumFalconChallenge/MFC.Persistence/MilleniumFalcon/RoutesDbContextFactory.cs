using Microsoft.EntityFrameworkCore;

namespace MFC.Persistence.MilleniumFalcon
{
    public class RoutesDbContextFactory : IDbContextFactory<RoutesDbContext>
    {
        private readonly string _pathToDb;

        public RoutesDbContextFactory(string pathToDb)
        {
            _pathToDb = pathToDb;
        }

        public RoutesDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<RoutesDbContext>()
                .UseSqlite($"Data Source={_pathToDb}")
                .Options;
            return new RoutesDbContext(options);
        }
    }
}