using MFC.Domain;
using Microsoft.EntityFrameworkCore;

namespace MFC.Persistence.MilleniumFalcon
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

        public Task<List<Route>> GetRoutesAsync(PlanetIdentifier planet)
        {
            using var context = _contextFactory.CreateDbContext();
            string planetName = planet.Name;
            return context
                .Routes
                .Where(r => r.Origin == planetName || r.Destination == planetName)
                .Select(r => new Route(r.Origin, r.Destination, r.TravelTime))
                .ToListAsync();
        }

        public Task<List<Route>> GetAllRoutesAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return context
                .Routes
                .Select(r => new Route(r.Origin, r.Destination, r.TravelTime))
                .ToListAsync();
        }
    }
}