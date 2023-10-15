using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using MFC.Domain;
using MFC.Domain.Runners;
using MFC.Persistence.Empire;
using MFC.Persistence.MilleniumFalcon;
using Microsoft.Extensions.Logging.Abstractions;

namespace MFC.Benchmark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [RPlotExporter]
    [MemoryDiagnoser]
    public class RunnerTests
    {
        private IReadOnlyRoutesRepository _routesRepository;
        private MilleniumFalconInformation _information;
        private EmpireConfiguration _empireConfiguration;

        [Params("queue", "hashset")]
        public string runners;

        [Params("example1", "example2", "example3", "example4")]
        public string example;

        [GlobalSetup]
        public void Setup()
        {
            MilleniumFalconConfiguration? conf =
                new MilleniumFalconConfigurationLoader(NullLoggerFactory.Instance)
                .Load(Path.Combine(Environment.CurrentDirectory, example, "millennium-falcon.json"));

            _information = new(conf.Autonomy, conf.Departure, conf.Arrival);
            _empireConfiguration =
                new EmpireConfigurationLoader(NullLoggerFactory.Instance)
                .Load(Path.Combine(Environment.CurrentDirectory, example, "empire.json"));
            _routesRepository = new RoutesRepository(new RoutesDbContextFactory(conf.RoutesDbPath));
        }

        [Benchmark]
        public async Task RunAsync()
        {
            IScenarioRunner runner;
            if (runners == "queue")
            {
                runner = new QueueScenarioRunner(_routesRepository, _information, NullLoggerFactory.Instance);
            }
            else
            {
                runner = new HashSetScenarioRunner(_routesRepository, _information, NullLoggerFactory.Instance);
            }
            var bountyHunters = _empireConfiguration.BountyHunters.Select(bh => new BountyHunter(bh.Planet, bh.Day)).ToArray();
            Scenario scenario = new(1, string.Empty, _empireConfiguration.Countdown, 0, bountyHunters);
            await runner.RunAsync(scenario);
        }
    }
}
