using Microsoft.EntityFrameworkCore;

namespace MilleniumFalconChallenge.Persistence.Scenarios
{
    public class ScenarioRepository : IScenarioRepository
    {
        private readonly IDbContextFactory<ScenarioDbContext> _contextFactory;

        public ScenarioRepository(IDbContextFactory<ScenarioDbContext> contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<int> CountAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Scenarios.CountAsync();
        }

        public async Task<Scenario?> GetAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var entity = await context.Scenarios.FindAsync(id);
            return entity?.ToDomain();
        }

        public async IAsyncEnumerable<Scenario> GetAsync(int page, int pageSize)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var asyncEnum = context.Scenarios
                .Include(x => x.BountyHunters)
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => e.ToDomain())
                .ToAsyncEnumerable();

            await foreach (var entity in asyncEnum)
            {
                yield return entity;
            }
        }

        public async Task<Scenario?> InsertAsync(NewScenario scenario)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var scenarioEntity = new ScenarioEntity
            {
                Name = scenario.Name,
                Countdown = scenario.Countdown
            };

            context.Scenarios.Add(scenarioEntity);

            var bountyHunterEntities = scenario.BountyHunters.Select(bh => new BountyHunterEntity
            {
                Day = bh.Day,
                Planet = bh.Planet,
                Scenario = scenarioEntity
            }).ToArray();

            await context.BountyHunters.AddRangeAsync(bountyHunterEntities);

            await context.SaveChangesAsync();

            return scenarioEntity.ToDomain();
        }

        public Task<bool> UpdateProbabilityAsync(Scenario scenario, double probability)
        {
            return Task.FromResult(false);
        }
    }
}