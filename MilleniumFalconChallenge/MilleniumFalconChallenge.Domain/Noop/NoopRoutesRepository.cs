namespace MilleniumFalconChallenge.Domain.Noop
{
    public class NoopRoutesRepository : IReadOnlyRoutesRepository
    {
        public Task<bool> DoesPlanetExistsAsync(PlanetIdentifier planet) => Task.FromResult(true);

        public Task<List<Route>> GetRoutesAsync(PlanetIdentifier planet) => Task.FromResult(new List<Route>());
    }
}
