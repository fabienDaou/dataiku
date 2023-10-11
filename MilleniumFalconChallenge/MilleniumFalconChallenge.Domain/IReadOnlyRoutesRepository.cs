namespace MilleniumFalconChallenge
{
    public interface IReadOnlyRoutesRepository
    {
        Task<bool> DoesPlanetExistsAsync(PlanetIdentifier planet);
    }
}
