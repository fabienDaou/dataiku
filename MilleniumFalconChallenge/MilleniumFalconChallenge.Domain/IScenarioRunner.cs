namespace MilleniumFalconChallenge
{
    public interface IScenarioRunner
    {
        Task<double> RunAsync(Scenario scenario);
    }
}
