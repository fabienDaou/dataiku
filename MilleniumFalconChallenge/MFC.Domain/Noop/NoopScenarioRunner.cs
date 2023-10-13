using Microsoft.Extensions.Logging;

namespace MFC.Domain.Noop
{
    public class NoopScenarioRunner : IScenarioRunner
    {
        private readonly ILogger _logger;
        public NoopScenarioRunner(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger<NoopScenarioRunner>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task<double> RunAsync(Scenario scenario)
        {
            _logger.LogInformation("Scenario '{Name}' run started.", scenario.Name);

            await Task.Delay(TimeSpan.FromSeconds(10));

            _logger.LogInformation("Scenario '{Name}' run completed.", scenario.Name);

            return new Random().Next(100);
        }
    }
}
