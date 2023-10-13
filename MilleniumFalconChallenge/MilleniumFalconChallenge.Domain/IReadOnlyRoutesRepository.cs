namespace MilleniumFalconChallenge
{
    public interface IReadOnlyRoutesRepository
    {
        Task<bool> DoesPlanetExistsAsync(PlanetIdentifier planet);

        Task<List<Route>> GetRoutesAsync(PlanetIdentifier planet);
    }
}
