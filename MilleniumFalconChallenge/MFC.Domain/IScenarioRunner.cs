namespace MFC.Domain
{
    public interface IScenarioRunner
    {
        Task<double> RunAsync(Scenario scenario);
    }
}
