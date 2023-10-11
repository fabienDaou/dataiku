namespace MilleniumFalconChallenge
{
    public interface IReadOnlyScenarioRepository
    {
        Task<Scenario?> GetAsync(int id);
        IAsyncEnumerable<Scenario> GetAsync(int page, int pageSize);
        Task<int> CountAsync();
    }
}
