using static MilleniumFalconChallenge.INewScenarioHandler;

namespace MilleniumFalconChallenge.Domain
{
    public class NewScenarioHandler : INewScenarioHandler
    {
        private readonly IScenarioProcessingDispatcher _scenarioProcessingDispatcher;
        private readonly IScenarioRepository _scenarioRepository;
        private readonly IReadOnlyRoutesRepository _routesRepository;

        public NewScenarioHandler(
            IScenarioProcessingDispatcher scenarioProcessingDispatcher,
            IScenarioRepository scenarioRepository,
            IReadOnlyRoutesRepository routesRepository)
        {
            _scenarioProcessingDispatcher = scenarioProcessingDispatcher ?? throw new ArgumentNullException(nameof(scenarioProcessingDispatcher));
            _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));
            _routesRepository = routesRepository ?? throw new ArgumentNullException(nameof(routesRepository));
        }

        public async Task<Result> HandleAsync(NewScenario scenario)
        {
            foreach (var planet in scenario.BountyHunters.Select(s => s.Planet))
            {
                if (!await _routesRepository.DoesPlanetExistsAsync(planet))
                {
                    return new InvalidScenario("Invalid planet.");
                }
            }

            var scenarioCreated = await _scenarioRepository.InsertAsync(scenario);
            if (scenarioCreated is not null)
            {
                _scenarioProcessingDispatcher.Dispatch(scenarioCreated);

                return new Success(scenarioCreated.Id);
            }
            else
            {
                return new UnexpectedError();
            }
        }
    }
}