namespace MFC.Domain.Noop
{
    public class NoopRoutesRepository : IReadOnlyRoutesRepository
    {
        public Task<bool> DoesPlanetExistsAsync(PlanetIdentifier planet) => Task.FromResult(true);

        public Task<List<Route>> GetAllRoutesAsync() => Task.FromResult(new List<Route>());

        public Task<List<Route>> GetRoutesAsync(PlanetIdentifier planet) => Task.FromResult(new List<Route>());
    }
}
