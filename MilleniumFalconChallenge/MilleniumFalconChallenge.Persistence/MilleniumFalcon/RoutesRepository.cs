using Microsoft.EntityFrameworkCore;

namespace MilleniumFalconChallenge.Persistence.MilleniumFalcon
{
    public class RoutesRepository : IReadOnlyRoutesRepository
    {
        private readonly IDbContextFactory<RoutesDbContext> _contextFactory;

        public RoutesRepository(IDbContextFactory<RoutesDbContext> contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<bool> DoesPlanetExistsAsync(PlanetIdentifier planet)
        {
            using var context = _contextFactory.CreateDbContext();
            string planetName = planet.Name;
            return await context.Routes.AnyAsync(r => r.Origin == planetName || r.Destination == planetName);
        }
    }
}