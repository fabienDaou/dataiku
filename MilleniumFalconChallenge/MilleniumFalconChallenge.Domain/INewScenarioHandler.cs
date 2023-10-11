namespace MilleniumFalconChallenge
{
    public interface INewScenarioHandler
    {
        Task<int?> HandleAsync(NewScenario scenario);
    }
}
