using Microsoft.Extensions.Logging;

namespace MilleniumFalconChallenge.Domain.Runners
{
    public class DijkstraScenarioRunner : IScenarioRunner
    {
        private readonly IReadOnlyRoutesRepository _routesRepository;
        private readonly MilleniumFalconInformation _milleniumFalconInformation;
        private readonly ILogger _logger;

        public DijkstraScenarioRunner(
            IReadOnlyRoutesRepository routesRepository,
            MilleniumFalconInformation milleniumFalconInformation,
            ILoggerFactory loggerFactory)
        {
            _routesRepository = routesRepository ?? throw new ArgumentNullException(nameof(routesRepository));
            _milleniumFalconInformation = milleniumFalconInformation ?? throw new ArgumentNullException(nameof(milleniumFalconInformation));
            _logger = loggerFactory?.CreateLogger<DijkstraScenarioRunner>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task<double> RunAsync(Scenario scenario)
        {
            _logger.LogInformation("Scenario '{Name}' run started with Dijkstra.", scenario.Name);

            await Task.Delay(TimeSpan.FromSeconds(10));

            _logger.LogInformation("Scenario '{Name}' run completed.", scenario.Name);

            return new Random().Next(100);
        }
    }
}
