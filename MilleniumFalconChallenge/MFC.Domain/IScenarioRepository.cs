namespace MFC.Domain
{
    public interface IScenarioRepository : IReadOnlyScenarioRepository
    {
        Task<Scenario?> InsertAsync(NewScenario scenario);
        Task<bool> UpdateProbabilityAsync(Scenario scenario, double probability);
    }
}
