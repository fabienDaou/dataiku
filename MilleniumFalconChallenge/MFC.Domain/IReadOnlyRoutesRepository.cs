namespace MFC.Domain
{
    public interface IReadOnlyRoutesRepository
    {
        Task<bool> DoesPlanetExistsAsync(PlanetIdentifier planet);

        Task<List<Route>> GetRoutesAsync(PlanetIdentifier planet);
        Task<List<Route>> GetAllRoutesAsync();
    }
}
