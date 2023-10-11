namespace MilleniumFalconChallenge.Domain.Noop
{
    public class NoopScenarioRepository : IScenarioRepository
    {
        public Task<int> CountAsync() => Task.FromResult(0);

        public Task<Scenario?> GetAsync(int id) => Task.FromResult<Scenario?>(null);

        public IAsyncEnumerable<Scenario> GetAsync(int page, int pageSize) => AsyncEnumerable.Empty<Scenario>();

        public Task<Scenario?> InsertAsync(NewScenario scenario)
        {
            var random = new Random().Next(100);
            return Task.FromResult<Scenario?>(new Scenario(random, scenario.Name, scenario.Countdown, null, scenario.BountyHunters));
        }

        public Task<bool> UpdateProbabilityAsync(Scenario scenario, double probability) => Task.FromResult(false);
    }
}
